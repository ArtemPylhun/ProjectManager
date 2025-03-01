using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ProjectTaskRepository(ApplicationDbContext context) : IProjectTaskRepository, IProjectTaskQueries
{
    public async Task<IReadOnlyList<ProjectTask>> GetAll(CancellationToken cancellationToken)
    {
        return await context.ProjectTasks
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectTask>> GetAllByProjectId(ProjectId projectId,
        CancellationToken cancellationToken)
    {
        return await context.ProjectTasks
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ProjectTask> ProjectTasks, int TotalCount)> GetAllPaginated(int page, int pageSize,
        string search, CancellationToken cancellationToken)
    {
        IQueryable<ProjectTask> query = context.ProjectTasks
            .AsNoTracking();

        query = query
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .OrderBy(x => x.Name);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();

            bool isStatusSearch = Enum.TryParse<ProjectTask.ProjectTaskStatuses>(search, true, out var status);

            query = query.Where(pt =>
                pt.Name.ToLower().Contains(searchLower) ||
                pt.Description.ToLower().Contains(searchLower) ||
                pt.Project.Name.ToLower().Contains(searchLower) ||
                pt.Creator.UserName.ToLower().Contains(searchLower) ||
                (isStatusSearch && pt.Status == status));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var projectTasks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (projectTasks, totalCount);
    }

    public async Task<(IReadOnlyList<ProjectTask> ProjectTasks, int TotalCount)> GetAllByUserIdPaginated(Guid userId,
        int page, int pageSize, string search, CancellationToken cancellationToken)
    {
        IQueryable<ProjectTask> query = context.ProjectTasks
            .AsNoTracking()
            .Where(x => x.CreatorId == userId);

        query = query
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .OrderBy(x => x.Name);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();

            bool isStatusSearch = Enum.TryParse<ProjectTask.ProjectTaskStatuses>(search, true, out var status);

            query = query.Where(pt =>
                pt.Name.ToLower().Contains(searchLower) ||
                pt.Description.ToLower().Contains(searchLower) ||
                pt.Project.Name.ToLower().Contains(searchLower) ||
                pt.Creator.UserName.ToLower().Contains(searchLower) ||
                (isStatusSearch && pt.Status == status));
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        var projectTasks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (projectTasks, totalCount);
    }

    public async Task<IReadOnlyList<ProjectTask>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await context.ProjectTasks
            .AsNoTracking()
            .Where(x => x.CreatorId == userId)
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ProjectTask>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.ProjectTasks
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<ProjectTask>() : Option.Some(entity);
    }

    public async Task<Option<ProjectTask>> GetById(ProjectTaskId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProjectTasks
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<ProjectTask>() : Option.Some(entity);
    }

    public async Task<ProjectTask> Add(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        await context.ProjectTasks.AddAsync(projectTask, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return await context.ProjectTasks
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .FirstAsync(x => x.Id == projectTask.Id, cancellationToken);
    }

    public async Task<ProjectTask> Update(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        var existingEntry = context.ProjectTasks.Find(projectTask.Id);
        if (existingEntry == null)
        {
            context.ProjectTasks.Add(projectTask);
        }
        else
        {
            context.Entry(existingEntry).CurrentValues.SetValues(projectTask); // Update tracked entity
        }

        await context.SaveChangesAsync(cancellationToken);

        return await context.ProjectTasks
            .Include(x => x.Project)
            .Include(x => x.Creator)
            .FirstAsync(x => x.Id == projectTask.Id, cancellationToken);
    }

    public async Task<ProjectTask> Delete(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        context.ProjectTasks.Remove(projectTask);

        await context.SaveChangesAsync(cancellationToken);

        return projectTask;
    }
}