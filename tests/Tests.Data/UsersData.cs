using Domain.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Tests.Data;

public static class UsersData
{
    private static readonly PasswordHasher<User> PasswordHasher = new PasswordHasher<User>();

    public static User AdminUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "adminN@gmail.com",
        UserName = "adminN",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    public static User NewUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "email@gmail.com",
        UserName = "username",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };

    public static User MainUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "userName@gmail.com",
        UserName = "UserName",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    public static User MainUser2 = new User
    {
        Id = Guid.NewGuid(),
        Email = "userName2@gmail.com",
        UserName = "UserName2",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    public static User UserForDeletion = new User
    {
        Id = Guid.NewGuid(),
        Email = "userName12@gmail.com",
        UserName = "UserName12",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    
    public static User MainUserForProject = new User
    {
        Id = Guid.NewGuid(),
        Email = "userNameForProject@gmail.com",
        UserName = "userNameForProject",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    public static User MainUserForProjectTask = new User
    {
        Id = Guid.NewGuid(),
        Email = "userNameForProjectTask@gmail.com",
        UserName = "userNameForProjectTask",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    public static User MainUserForProjectTask2 = new User
    {
        Id = Guid.NewGuid(),
        Email = "userNameForProjectTask2@gmail.com",
        UserName = "userNameForProjectTask2",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    
    public static User MainUserForTimeEntry = new User
    {
        Id = Guid.NewGuid(),
        Email = "userNameForTimeEntry@gmail.com",
        UserName = "userNameForTimeEntry",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    public static User MainUserForTimeEntry2 = new User
    {
        Id = Guid.NewGuid(),
        Email = "userNameForTimeEntry2@gmail.com",
        UserName = "userNameForTimeEntry2",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
    public static User MainUserForProjectUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "userNameForProjectUser@gmail.com",
        UserName = "userNameForProjectUser",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true
    };
    
}