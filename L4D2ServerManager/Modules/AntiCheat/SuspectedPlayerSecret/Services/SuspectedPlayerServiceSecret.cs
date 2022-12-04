using Azure.Data.Tables;
using FluentValidation;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Commands;

namespace L4D2ServerManager.Modules.AntiCheat.SuspectedPlayerSecret.Services;

public class SuspectedPlayerServiceSecret : ISuspectedPlayerServiceSecret
{
    private static bool _created;
    private readonly IValidator<AddSuspectedPlayerSecretCommand> _commandValidator;
    private readonly IAzureTableStorageContext _tableContext;
    private readonly IValidator<SuspectedPlayerSecret> _validator;
    private TableClient? _suspectedPlayerSecretTable;

    public SuspectedPlayerServiceSecret(IAzureTableStorageContext tableContext,
        IValidator<SuspectedPlayerSecret> validator,
        IValidator<AddSuspectedPlayerSecretCommand> commandValidator)
    {
        _tableContext = tableContext;
        _validator = validator;
        _commandValidator = commandValidator;

        CreateIfNotExists();
    }

    private TableClient SuspectedPlayerSecretTable => _suspectedPlayerSecretTable ??= _tableContext.GetTableClient("SuspectedPlayerSecret").Result;

    public SuspectedPlayerSecret Add(AddSuspectedPlayerSecretCommand command)
    {
        _commandValidator.ValidateAndThrowAsync(command).Wait();

        var suspectedPlayer = new SuspectedPlayerSecret
        {
            CommunityId = command.CommunityId ?? 0
        };

        _validator.ValidateAndThrowAsync(suspectedPlayer).Wait();

        SuspectedPlayerSecretTable.AddEntity(suspectedPlayer);

        return suspectedPlayer;
    }

    private void CreateIfNotExists()
    {
        if (_created)
            return;

        SuspectedPlayerSecretTable.CreateIfNotExists();

        _created = true;
    }
}