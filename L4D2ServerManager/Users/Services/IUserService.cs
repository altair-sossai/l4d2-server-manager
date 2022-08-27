namespace L4D2ServerManager.Users.Services;

public interface IUserService
{
    User EnsureAuthentication(string token);
    List<User> GetUsers();
}