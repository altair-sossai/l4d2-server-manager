using L4D2ServerManager.Modules.Auth.Users.Enums;
using L4D2ServerManager.Modules.ServerManager.Server;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine;

namespace L4D2ServerManager.Modules.Auth.Users.Services;

public interface IUserService
{
    User? GetUser(string userId);
    User EnsureAuthentication(string token);
    User EnsureAuthentication(string token, AccessLevel accessLevel);
    void ApplyPermissions(User user, IVirtualMachine virtualMachine);
    void ApplyPermissions(User user, IServer server);
}