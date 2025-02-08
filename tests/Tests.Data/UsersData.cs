using Domain.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Tests.Data;

public static class UsersData
{
    private static readonly PasswordHasher<User> PasswordHasher = new PasswordHasher<User>();

    public static User NewUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "email@gmail.com",
        UserName = "username",
        NormalizedUserName = "username".ToUpperInvariant(),
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        NormalizedEmail = "email@gmail.com".ToUpperInvariant(),
        SecurityStamp = Guid.NewGuid().ToString()
    };

    public static User MainUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "userName@gmail.com",
        UserName = "UserName",
        NormalizedUserName = "UserName".ToUpperInvariant(),
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        NormalizedEmail = "userName@gmail.com".ToUpperInvariant(),
        SecurityStamp = Guid.NewGuid().ToString()
    };
    
    public static User MainUser2 = new User
    {
        Id = Guid.NewGuid(),
        Email = "userName2@gmail.com",
        UserName = "UserName2",
        NormalizedUserName = "UserName2".ToUpperInvariant(),
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        NormalizedEmail = "userName2@gmail.com".ToUpperInvariant(),
        SecurityStamp = Guid.NewGuid().ToString()
    };
    
    public static User UserForDeletion = new User
    {
        Id = Guid.NewGuid(),
        Email = "userName12@gmail.com",
        UserName = "UserName12",
        NormalizedUserName = "UserName12".ToUpperInvariant(),
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        NormalizedEmail = "userName12@gmail.com".ToUpperInvariant(),
        SecurityStamp = Guid.NewGuid().ToString()
    };
    
}