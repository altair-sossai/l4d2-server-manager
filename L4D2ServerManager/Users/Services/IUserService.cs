using L4D2ServerManager.VirtualMachine;

namespace L4D2ServerManager.Users.Services;

public interface IUserService
{
    User EnsureAuthentication(string token);
    User? GetUser(string userId);
    IEnumerable<User> GetUsers();
    void ApplyPermissions(User user, IVirtualMachine virtualMachine);
}