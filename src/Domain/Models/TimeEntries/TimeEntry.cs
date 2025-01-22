using Domain.Models.Projects;
using Domain.Models.Users;

namespace Domain.Models.TimeEntries;

public class TimeEntry
{
    public TimeEntryId Id { get; set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Hours { get; private set; }

    /*public Guid UserId { get; private set;}
    public User? User { get; }*/

    public ProjectId ProjectId { get; private set; }
    public Project? Project { get; }

    /*public TaskId TaskId { get; private set; }
    public Task? Task { get; }*/

    private TimeEntry(TimeEntryId id, string description, DateTime startDate, DateTime endDate, int hours,
        ProjectId projectId)
    {
        Id = id;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Hours = hours;
        ProjectId = projectId;
    }

    public static TimeEntry New(TimeEntryId id, string description, DateTime startDate, DateTime endDate, int hours,
        ProjectId projectId) => new(id, description, startDate, endDate, hours, projectId);


    /*private TimeEntry(TimeEntryId id, string description, DateTime startDate, DateTime endDate, int hours,
        Guid userId, ProjectId projectId, TaskId taskId)
    {
        Id = id;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Hours = hours;
        UserId = userId;
        ProjectId = projectId;
        TaskId = taskId;
    }

    public static TimeEntry New(TimeEntryId id, string description, DateTime startDate, DateTime endDate, int hours,
        Guid userId, ProjectId projectId, TaskId taskId) => new(id,description, startDate, endDate, hours, userId, projectId, taskId);*/
}