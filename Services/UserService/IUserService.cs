using Entities;

public interface IUserService
{
    User ValidateUser(string email, string password);
    User GetUserById(int id);
}