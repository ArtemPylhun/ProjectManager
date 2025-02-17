using Domain.Models.ProjectTasks;

namespace API.DTOs;

public record ProjectTaskDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    int EstimatedTime,
    string Description,
    ProjectDto? Project,
    ProjectTask.ProjectTaskStatuses Status,
    IList<UserTaskDto>? UsersTask)
{
public static ProjectTaskDto FromDomainModel(
    ProjectTask projectTask)
    => new(
        Id: projectTask.Id.Value,
        ProjectId: projectTask.ProjectId.Value,
        Name: projectTask.Name,
        EstimatedTime: projectTask.EstimatedTime,
        Description: projectTask.Description,
        Project: ProjectDto.FromDomainModel(projectTask.Project),
        Status: projectTask.Status,
        UsersTask: projectTask.UsersTask.Select(UserTaskDto.FromDomainModel).ToList());
}

public record ProjectTaskCreateDto(
    Guid ProjectId,
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