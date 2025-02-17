using Domain.Models.ProjectUsers;

namespace API.DTOs;

public record ProjectUserDto(
    Guid Id,
    Guid ProjectId,
    Guid RoleId,
    Guid UserId)
{
    public static ProjectUserDto FromDomainModel(ProjectUser projectUser)
        => new(
            Id: projectUser.Id.Value,
            ProjectId: projectUser.ProjectId.Value,
            RoleId: projectUser.RoleId,
            UserId: projectUser.UserId);
}

public record ProjectUserCreateDto(
    Guid ProjectId,
    Guid RoleId,
    Guid UserId);
