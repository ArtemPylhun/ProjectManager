using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.Users;

namespace Domain.Models.TimeEntries;

public class TimeEntry
{
    public TimeEntryId Id { get; set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Hours { get; private set; }

    public Guid UserId { get; private set; }
    public User? User { get; }

    public ProjectId ProjectId { get; private set; }
    public Project? Project { get; }

    public ProjectTaskId? ProjectTaskId { get; private set; }
    public ProjectTask? ProjectTask { get; }


    private TimeEntry(TimeEntryId id, string description, DateTime startDate, DateTime endDate, int hours,
        Guid userId, ProjectId projectId, ProjectTaskId? projectTaskId)
    {
        Id = id;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Hours = hours;
        UserId = userId;
        ProjectId = projectId;
        ProjectTaskId = projectTaskId;
    }

    public static TimeEntry New(TimeEntryId id, string description, DateTime startDate, DateTime endDate, int hours,
        Guid userId, ProjectId projectId, ProjectTaskId projectTaskId) => new(id, description, startDate, endDate,
        hours, userId, projectId, projectTaskId);

    public void UpdateDetails(string description, DateTime startTime, DateTime endTime, int hours, Guid userId,
        ProjectId projectId, ProjectTaskId projectTaskId)
    {
        Description = description;
        StartDate = startTime;
        EndDate = endTime;
        Hours = hours;
        UserId = userId;
        ProjectId = projectId;
        ProjectTaskId = projectTaskId;
    }
}