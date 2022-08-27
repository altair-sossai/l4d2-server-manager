using L4D2ServerManager.Users.Commands;

namespace L4D2ServerManager.Users.Services;

public interface IUserService
{
    User? Authenticate(AuthenticationCommand command);
    List<User> GetUsers();
}