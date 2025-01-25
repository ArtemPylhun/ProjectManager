using Domain.Models.Users;

namespace API.DTOs;

public record UserDto(
    Guid? Id,
    string? Email,
    string? UserName,
    string? Password)
{
    public static UserDto FromDomainModel(User user)
        => new(
            Id: user.Id,
            Email: user.Email,
            UserName: user.UserName,
            Password: user.PasswordHash);
}

public record UserCreateDto(
    string Email,
    string UserName,
    string Password)
{
    public static UserCreateDto FromUserCreateDomainModel(User user)
        => new(
            Email: user.Email,
            UserName: user.UserName,
            Password: user.PasswordHash
        );
}