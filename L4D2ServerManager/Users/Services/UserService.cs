using Azure.Data.Tables;
using L4D2ServerManager.Contexts.AzureTableStorage;
using L4D2ServerManager.Users.Commands;

namespace L4D2ServerManager.Users.Services;

public class UserService : IUserService
{
    private readonly IAzureTableStorageContext _context;
    private TableClient? _userTable;

    public UserService(IAzureTableStorageContext context)
    {
        _context = context;
    }

    private TableClient UserTable => _userTable ??= _context.GetTableClient("Users").Result;

    public User EnsureAuthentication(string token)
    {
        var command = new AuthenticationCommand(token);
        if (!command.Valid)
            throw new UnauthorizedAccessException();

        var user = UserTable.Query<User>(user => user.Id == command.UserId && user.Secret == command.UserSecret).FirstOrDefault();
        if (user == null)
            throw new UnauthorizedAccessException();

        return user;
    }

    public List<User> GetUsers()
    {
        return UserTable.Query<User>().ToList();
    }
}