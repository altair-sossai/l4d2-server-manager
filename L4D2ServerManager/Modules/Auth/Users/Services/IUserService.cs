using L4D2ServerManager.Modules.ServerManager.Server;
using L4D2ServerManager.Modules.ServerManager.VirtualMachine;

namespace L4D2ServerManager.Modules.Auth.Users.Services;

public interface IUserService
{
    User EnsureAuthentication(string token);
    User? GetUser(string userId);
    IEnumerable<User> GetUsers();
    void ApplyPermissions(User user, IVirtualMachine virtualMachine);
    void ApplyPermissions(User user, IServer server);
}