using Domain.Models.UsersTasks;

namespace API.DTOs;

public record UserTaskDto(
    Guid Id,
    Guid ProjectTaskId,
    Guid UserId)
{
    public static UserTaskDto FromDomainModel(UserTask userTask)
        => new(
            Id: userTask.Id.Value,
            ProjectTaskId: userTask.ProjectTaskId.Value,
            UserId: userTask.UserId);
}

public record UserTaskCreateDto(
    Guid ProjectTaskId,
    Guid UserId);
