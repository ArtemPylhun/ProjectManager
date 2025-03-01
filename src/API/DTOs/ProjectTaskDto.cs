using Domain.Models.ProjectTasks;

namespace API.DTOs;

public record ProjectTaskDto(
    Guid Id,
    Guid ProjectId,
    Guid CreatorId,
    DateTime CreatedAt,
    string Name,
    int EstimatedTime,
    string Description,
    ProjectDto? Project,
    ProjectTask.ProjectTaskStatuses Status,
    UserDto? Creator)
{
    public static ProjectTaskDto FromDomainModel(
        ProjectTask projectTask)
        => new(
            Id: projectTask.Id.Value,
            ProjectId: projectTask.ProjectId.Value,
            CreatorId: projectTask.CreatorId,
            CreatedAt: projectTask.CreatedAt,
            Name: projectTask.Name,
            EstimatedTime: projectTask.EstimatedTime,
            Description: projectTask.Description,
            Project: projectTask.Project == null ? null : ProjectDto.FromDomainModel(projectTask.Project),
            Status: projectTask.Status,
            Creator: projectTask.Creator == null ? null : UserDto.FromDomainModel(projectTask.Creator));
}

public record ProjectTaskCreateDto(
    Guid ProjectId,
    Guid CreatorId,
    string Name,
    int EstimatedTime,
    string Description);

public record ProjectTaskUpdateDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    int EstimatedTime,
    string Description,
    ProjectTask.ProjectTaskStatuses Status);