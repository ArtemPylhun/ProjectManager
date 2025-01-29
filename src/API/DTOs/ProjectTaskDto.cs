using Domain.Models.Projects;
using Domain.Models.ProjectTasks;

namespace API.DTOs;

public record ProjectTaskDto(
    ProjectTaskId Id,
    ProjectId ProjectId,
    string Name,
    int EstimatedTime)
{
    public static ProjectTaskDto FromDomainModel(
        ProjectTask projectTask) 
        => new(
            Id: projectTask.Id,
            ProjectId:projectTask.ProjectId,
            Name: projectTask.Name, 
            EstimatedTime: projectTask.EstimatedTime
            );
}

public record ProjectTaskCreateDto(
    ProjectId ProjectId,
    string Name,
    int EstimatedTime)
{
}

public record ProjectTaskUpdateDto(
    ProjectTaskId? Id,
    ProjectId ProjectId,
    string Name,
    int EstimatedTime)
{
}