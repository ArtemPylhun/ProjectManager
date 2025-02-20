using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;

namespace API.DTOs;

public record TimeEntryDto(
    Guid Id,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    int Minutes,
    Guid UserId,
    UserDto? User,
    Guid ProjectId,
    ProjectDto? Project,
    Guid? ProjectTaskId,
    ProjectTaskDto? ProjectTask
)
{
    public static TimeEntryDto FromDomainModel(TimeEntry timeEntry) => new(
        Id: timeEntry.Id.Value,
        Description: timeEntry.Description,
        StartTime: timeEntry.StartDate,
        EndTime: timeEntry.EndDate,
        Minutes: timeEntry.Minutes,
        UserId: timeEntry.UserId,
        User: timeEntry.User == null ? null : UserDto.FromDomainModel(timeEntry.User),
        ProjectId: timeEntry.ProjectId.Value,
        Project: timeEntry.Project == null ? null : ProjectDto.FromDomainModel(timeEntry.Project),
        ProjectTaskId: timeEntry.ProjectTaskId!.Value,
        ProjectTask: timeEntry.ProjectTask == null ? null : ProjectTaskDto.FromDomainModel(timeEntry.ProjectTask));
}

public record TimeEntryCreateDto(
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    int Minutes,
    Guid UserId,
    Guid ProjectId,
    Guid? ProjectTaskId);

public record TimeEntryUpdateDto(
    Guid? Id,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    int Minutes,
    Guid UserId,
    Guid ProjectId,
    Guid? ProjectTaskId);
    
