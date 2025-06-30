using Models;
using System.Collections.Generic;
using System.Linq;
using Entities;

public class UserService : IUserService
{
    private readonly List<User> _users = new List<User>
    {
        new User { Id = 1, Email = "test@test.com", Password = "1234", RoleId = 1 },
        new User { Id = 2, Email = "user@test.com", Password = "1234", RoleId = 2 }
    };

    public User ValidateUser(string email, string password)
    {
        return _users.FirstOrDefault(u => u.Email == email && u.Password == password);
    }

    public User GetUserById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }
}