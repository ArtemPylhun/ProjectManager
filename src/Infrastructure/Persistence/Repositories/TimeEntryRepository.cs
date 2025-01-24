using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class TimeEntryRepository(ApplicationDbContext context): ITimeEntryQueries, ITimeEntryRepository
{
    public async Task<IReadOnlyList<TimeEntry>> GetAll(CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllByProjectTaskId(ProjectTaskId projectTaskId, CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .AsNoTracking()
            .Where(x => x.ProjectTaskId == projectTaskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await context.TimeEntries
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<TimeEntry>> GetById(TimeEntryId id, CancellationToken cancellationToken)
    {
        var entity =  await context.TimeEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option.None<TimeEntry>() : Option.Some(entity);
    }

    public async Task<Option<TimeEntry>> GetByProjectTaskAndProjectAndUserIds(ProjectTaskId projectTaskId, ProjectId projectId, Guid userId,
        CancellationToken cancellationToken)
    {
        var entity =  await context.TimeEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProjectTaskId == projectTaskId && x.ProjectId == projectId && x.UserId == userId, cancellationToken);
        
        return entity == null ? Option.None<TimeEntry>() : Option.Some(entity);
    }

    public async Task<TimeEntry> Add(TimeEntry timeEntry, CancellationToken cancellationToken)
    {
         await context.TimeEntries.AddAsync(timeEntry, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return timeEntry;
    }

    public async Task<TimeEntry> Update(TimeEntry timeEntry, CancellationToken cancellationToken)
    {
        context.TimeEntries.Update(timeEntry);

        await context.SaveChangesAsync(cancellationToken);

        return timeEntry;
    }

    public async Task<TimeEntry> Delete(TimeEntry timeEntry, CancellationToken cancellationToken)
    {
         context.TimeEntries.Remove(timeEntry);

        await context.SaveChangesAsync(cancellationToken);

        return timeEntry;
    }
}