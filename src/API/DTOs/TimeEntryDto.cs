using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;

namespace API.DTOs;

public record TimeEntryDto(
    TimeEntryId Id,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    int Minutes,
    Guid UserId,
    ProjectId ProjectId,
    ProjectTaskId? ProjectTaskId
)
{
    public static TimeEntryDto FromDomainModel(TimeEntry timeEntry) => new(
        timeEntry.Id,
        timeEntry.Description,
        timeEntry.StartDate,
        timeEntry.EndDate,
        timeEntry.Minutes,
        timeEntry.UserId,
        timeEntry.ProjectId,
        timeEntry.ProjectTaskId);
}

public record TimeEntryCreateDto(
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    int Minutes,
    Guid UserId,
    ProjectId ProjectId,
    ProjectTaskId? ProjectTaskId);

public record TimeEntryUpdateDto(
    TimeEntryId? Id,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    int Minutes,
    Guid UserId,
    ProjectId ProjectId,
    ProjectTaskId? ProjectTaskId);
    
