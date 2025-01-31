using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;

namespace Tests.Data;

public static class TimeEntriesData
{
    private static readonly DateTime _date = DateTime.UtcNow;
    public static TimeEntry NewTimeEntry(Guid userId, ProjectId projectId, ProjectTaskId? projectTaskId)
        =>
            TimeEntry.New(
                TimeEntryId.New(),
                "NewTimeEntry",
                _date,
                _date.AddDays(2).AddMinutes(2),
                1,
                userId,
                projectId,
                projectTaskId
            );
}