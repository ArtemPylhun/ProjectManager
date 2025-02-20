using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ProjectTaskRepository(ApplicationDbContext context): IProjectTaskRepository, IProjectTaskQueries
{
    public async Task<IReadOnlyList<ProjectTask>> GetAll(CancellationToken cancellationToken)
    {
        return await context.ProjectTasks
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.UsersTask)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectTask>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken)
    {
        return await context.ProjectTasks
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .Include(x => x.Project)
            .Include(x => x.UsersTask)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ProjectTask>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.ProjectTasks
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.UsersTask)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<ProjectTask>() : Option.Some(entity);
    }

    public async Task<Option<ProjectTask>> GetById(ProjectTaskId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProjectTasks
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.UsersTask)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<ProjectTask>() : Option.Some(entity);
    }

    public async Task<ProjectTask> Add(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        await context.ProjectTasks.AddAsync(projectTask, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return await context.ProjectTasks
            .Include(x => x.Project)
            .Include(x => x.UsersTask)
            .FirstAsync(x => x.Id == projectTask.Id, cancellationToken);
    }

    public async Task<ProjectTask> Update(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        context.ProjectTasks.Update(projectTask);

        await context.SaveChangesAsync(cancellationToken);

        return await context.ProjectTasks
            .Include(x => x.Project)
            .Include(x => x.UsersTask)
            .FirstAsync(x => x.Id == projectTask.Id, cancellationToken);
    }

    public async Task<ProjectTask> Delete(ProjectTask projectTask, CancellationToken cancellationToken)
    {
        context.ProjectTasks.Remove(projectTask);

        await context.SaveChangesAsync(cancellationToken);

        return projectTask;
    }
}