using Domain.Models.Roles;

namespace API.DTOs;

public record RoleDto(
    Guid? Id,
    string Name)
{
    public static RoleDto FromDomainModel(Role role)
        => new(
            Id: role.Id,
            Name: role.Name);
}