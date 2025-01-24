using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
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
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectTask>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken)
    {
        return await context.ProjectTasks
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ProjectTask>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.ProjectTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<ProjectTask>() : Option.Some(entity);
    }

    public async Task<Option<ProjectTask>> GetById(ProjectTaskId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProjectTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<ProjectTask>() : Option.Some(entity);
    }
    public async Task<Option<ProjectTask>> GetByProjectId(ProjectId id, CancellationToken cancellationToken)
    {
        var entity = await context.ProjectTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProjectId == id, cancellationToken);

        return entity == null ? Option.None<ProjectTask>() : Option.Some(entity);
    }

    public async Task<ProjectTask> Add(ProjectTask project, CancellationToken cancellationToken)
    {
        await context.ProjectTasks.AddAsync(project, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<ProjectTask> Update(ProjectTask project, CancellationToken cancellationToken)
    {
        context.ProjectTasks.Update(project);

        await context.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<ProjectTask> Delete(ProjectTask project, CancellationToken cancellationToken)
    {
        context.ProjectTasks.Remove(project);

        await context.SaveChangesAsync(cancellationToken);

        return project;
    }
}