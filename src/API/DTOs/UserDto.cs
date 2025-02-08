using Domain.Models.Users;

namespace API.DTOs;

public record UserDto(
    Guid Id,
    string UserName,
    string Email)
{
    public static UserDto FromDomainModel(User user)
        => new(user.Id, user.UserName, user.Email);
}

public record UserCreateDto(
    string Email,
    string UserName,
    string Password)
{
}

public record UserUpdateDto(
    Guid Id,
    string Email,
    string UserName,
    string Password)
{ }

public record UserLoginDto(
    string EmailOrUsername,
    string Password)
{
}

public record UserWithRolesDto(
    Guid Id,
    string UserName,
    string Email,
    IList<string> Roles)
{
    public static UserWithRolesDto FromDomainModel(User user, IList<string> roles)
        => new(user.Id, user.UserName, user.Email, roles);
}