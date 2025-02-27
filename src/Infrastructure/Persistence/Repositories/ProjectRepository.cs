using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository(ApplicationDbContext context) : IProjectQueries, IProjectRepository
{
    public async Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetAllPaginated(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = context.Projects
            .AsNoTracking()
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers);

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (projects, totalCount);
    }

    public async Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetAllByUserIdPaginated(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = context.Projects
            .AsNoTracking()
            .Where(x => x.ProjectUsers.Any(pu => pu.UserId == userId))
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers);

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (projects, totalCount);
    }
    
    public async Task<IReadOnlyList<Project>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Projects
            .AsNoTracking()
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyList<Project>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Projects
            .AsNoTracking()
            .Where(x => x.ProjectUsers.Any(pu => pu.UserId == userId))
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyList<Project>> GetAllByClient(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Projects
            .AsNoTracking()
            .Where(x => x.ClientId == userId)
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
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Project>() : Option.Some(entity);
    }

    public async Task<Option<Project>> GetById(ProjectId id, CancellationToken cancellationToken)
    {
        var entity = await context.Projects
            .AsNoTracking()
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Project>() : Option.Some(entity);
    }

    public async Task<Project> Add(Project project, CancellationToken cancellationToken)
    {
        await context.Projects.AddAsync(project, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return await context.Projects
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .FirstAsync(x => x.Id == project.Id, cancellationToken);
    }

    public async Task<Project> Update(Project project, CancellationToken cancellationToken)
    {
        var existingEntry = context.Projects.Find(project.Id);
        if (existingEntry == null)
        {
            context.Projects.Add(project); // If not found, add as new
        }
        else
        {
            context.Entry(existingEntry).CurrentValues.SetValues(project); // Update tracked entity
        }

        await context.SaveChangesAsync(cancellationToken);

        return await context.Projects
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .FirstAsync(x => x.Id == project.Id, cancellationToken);
    }

    public async Task<Project> Delete(Project project, CancellationToken cancellationToken)
    {
        context.Projects.Remove(project);

        await context.SaveChangesAsync(cancellationToken);

        return project;
    }
}