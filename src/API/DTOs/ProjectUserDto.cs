using Domain.Models.ProjectUsers;

namespace API.DTOs;

public record ProjectUserDto(
    Guid Id,
    Guid RoleId,
    Guid ProjectId,
    Guid UserId)
{
    public static ProjectUserDto FromDomainModel(ProjectUser projectUser)
        => new(
            Id: projectUser.Id.Value,
            RoleId: projectUser.RoleId,
            ProjectId: projectUser.ProjectId.Value,
            UserId: projectUser.UserId);
}

public record ProjectUserCreateDto(
    Guid RoleId,
    Guid ProjectId,
    Guid UserId);
