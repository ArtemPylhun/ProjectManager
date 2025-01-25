using Domain.Models.Users;

namespace Tests.Data;

public static class UsersData
{
    public static User NewUser = new User{ Id = Guid.NewGuid(), Email = "email@gmail.com", UserName = "username" };
    public static User MainUser = new User{ Id = Guid.NewGuid(), Email = "userName@gmail.com", UserName = "UserName" };
}