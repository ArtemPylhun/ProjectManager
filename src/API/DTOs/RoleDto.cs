using Domain.Models.Roles;

namespace API.DTOs;

public record RoleDto(
    Guid? Id,
    string Name,
    RoleGroups RoleGroup)
{
    public static RoleDto FromDomainModel(Role role)
        => new(
            Id: role.Id,
            Name: role.Name,
            RoleGroup: role.RoleGroup);
}