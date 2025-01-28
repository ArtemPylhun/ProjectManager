using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository(ApplicationDbContext context) : IProjectQueries, IProjectRepository
{
    public async Task<IReadOnlyList<Project>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Projects
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAllByCreator(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Projects
            .AsNoTracking()
            .Where(x => x.CreatorId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Project>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Project>() : Option.Some(entity);
    }

    public async Task<Option<Project>> GetById(ProjectId id, CancellationToken cancellationToken)
    {
        var entity = await context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Project>() : Option.Some(entity);
    }

    public async Task<Project> Add(Project project, CancellationToken cancellationToken)
    {
        await context.Projects.AddAsync(project, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<Project> Update(Project project, CancellationToken cancellationToken)
    {
        context.Projects.Update(project);

        await context.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<Project> Delete(Project project, CancellationToken cancellationToken)
    {
        context.Projects.Remove(project);

        await context.SaveChangesAsync(cancellationToken);

        return project;
    }
}