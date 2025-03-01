using Domain.Models.TimeEntries;

namespace Application.Common.Interfaces.Repositories;

public interface ITimeEntryRepository
{
    Task<TimeEntry> Add(TimeEntry timeEntry, CancellationToken cancellationToken);
    Task<TimeEntry> Update(TimeEntry timeEntry, CancellationToken cancellationToken);
    Task<TimeEntry> Delete(TimeEntry timeEntry, CancellationToken cancellationToken);

    Task<bool> HasTimeOverlap(Guid userId, DateTime startTime, DateTime? endTime, TimeEntryId? excludeId = null,
        CancellationToken cancellationToken = default);
}