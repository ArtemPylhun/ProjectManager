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
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        NormalizedEmail = "email@gmail.com".ToUpperInvariant(),
        SecurityStamp = Guid.NewGuid().ToString()
    };

    public static User MainUser = new User
    {
        Id = Guid.NewGuid(),
        Email = "userName@gmail.com",
        UserName = "UserName",
        PasswordHash = PasswordHasher.HashPassword(null, "Admin!23"),
        NormalizedEmail = "userName@gmail.com".ToUpperInvariant(),
        SecurityStamp = Guid.NewGuid().ToString()
    };}