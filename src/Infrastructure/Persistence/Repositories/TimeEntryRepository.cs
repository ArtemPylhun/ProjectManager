using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class TimeEntryRepository(ApplicationDbContext context) : ITimeEntryQueries, ITimeEntryRepository
{
    public async Task<IReadOnlyList<TimeEntry>> GetAll(CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllByProjectTaskId(ProjectTaskId projectTaskId,
        CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .Where(x => x.ProjectTaskId == projectTaskId)
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllByProjectId(ProjectId projectId,
        CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .Where(x => x.ProjectId == projectId)
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .Where(x => x.UserId == userId)
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<TimeEntry>> GetById(TimeEntryId id, CancellationToken cancellationToken)
    {
        var entity = await context.TimeEntries
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<TimeEntry>() : Option.Some(entity);
    }

    public async Task<Option<TimeEntry>> GetByProjectTaskAndProjectAndUserIds(ProjectTaskId projectTaskId,
        ProjectId projectId, Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = await context.TimeEntries
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.ProjectTaskId == projectTaskId && x.ProjectId == projectId && x.UserId == userId,
                cancellationToken);

        return entity == null ? Option.None<TimeEntry>() : Option.Some(entity);
    }
    
    public async Task<IReadOnlyList<TimeEntry>> GetDailyTimeEntriesForUser(Guid userId, DateTime date, CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .Where(x => x.UserId == userId && x.StartDate.Date == date.Date)
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TimeEntry> Add(TimeEntry timeEntry, CancellationToken cancellationToken)
    {
        await context.TimeEntries.AddAsync(timeEntry, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return await context.TimeEntries
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .FirstAsync(x => x.Id == timeEntry.Id, cancellationToken);
    }

    public async Task<TimeEntry> Update(TimeEntry timeEntry, CancellationToken cancellationToken)
    {
        var existingEntry = context.TimeEntries.Find(timeEntry.Id);
        if (existingEntry == null)
        {
            context.TimeEntries.Add(timeEntry); // If not found, add as new
        }
        else
        {
            context.Entry(existingEntry).CurrentValues.SetValues(timeEntry); // Update tracked entity
        }

        await context.SaveChangesAsync(cancellationToken);

        // Reload the entity to ensure it reflects the database state
        return await context.TimeEntries
            .Include(x => x.Project)
            .Include(x => x.ProjectTask)
            .Include(x => x.User)
            .FirstAsync(x => x.Id == timeEntry.Id, cancellationToken);
    }

    public async Task<TimeEntry> Delete(TimeEntry timeEntry, CancellationToken cancellationToken)
    {
        context.TimeEntries.Remove(timeEntry);

        await context.SaveChangesAsync(cancellationToken);

        return timeEntry;
    }
}