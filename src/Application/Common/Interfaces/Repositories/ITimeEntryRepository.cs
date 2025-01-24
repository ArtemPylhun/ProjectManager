using Domain.Models.TimeEntries;
using Domain.Models.UsersTasks;

namespace Application.Common.Interfaces.Repositories;

public interface ITimeEntryRepository
{
    Task<TimeEntry> Add(TimeEntry timeEntry, CancellationToken cancellationToken);
    Task<TimeEntry> Update(TimeEntry timeEntry, CancellationToken cancellationToken);
    Task<TimeEntry> Delete(TimeEntry timeEntry, CancellationToken cancellationToken);
}