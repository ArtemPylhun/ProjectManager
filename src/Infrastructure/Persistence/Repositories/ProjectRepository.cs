using System.Security.Cryptography.Xml;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository(ApplicationDbContext context) : IProjectQueries, IProjectRepository
{
    public async Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetAllPaginated(int page, int pageSize,
        string search, CancellationToken cancellationToken)
    {
        var query = context.Projects
            .OrderBy(x => x.Name)
            .AsNoTracking();

        query = query
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.ToLower().Contains(search.ToLower()) ||
                p.Description.ToLower().Contains(search.ToLower()) ||
                p.Creator.UserName.ToLower().Contains(search.ToLower()) ||
                p.Client.UserName.ToLower().Contains(search.ToLower()) ||
                p.ProjectUsers.Any(pu => pu.User.UserName.ToLower().Contains(search.ToLower())) ||
                p.ProjectUsers.Any(pu => pu.Role.Name.ToLower().Contains(search.ToLower())));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (projects, totalCount);
    }

    public async Task<(IReadOnlyList<Project> Projects, int TotalCount)> GetAllByUserIdPaginated(Guid userId, int page,
        int pageSize, string search, CancellationToken cancellationToken)
    {
        var query = context.Projects
            .AsNoTracking()
            .Where(x => x.ProjectUsers.Any(pu => pu.UserId == userId));

        query = query
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .OrderBy(x => x.Name);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.ToLower().Contains(search.ToLower()) ||
                p.Description.ToLower().Contains(search.ToLower()) ||
                p.Creator.UserName.ToLower().Contains(search.ToLower()) ||
                p.Client.UserName.ToLower().Contains(search.ToLower()) ||
                p.ProjectUsers.Any(pu => pu.User.UserName.ToLower().Contains(search.ToLower())) ||
                p.ProjectUsers.Any(pu => pu.Role.Name.ToLower().Contains(search.ToLower())));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (projects, totalCount);
    }

    public async Task<IReadOnlyList<Guid>> GetAllUsersByProjectId(Guid projId, CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(projId);
        return await context.Projects
            .AsNoTracking()
            .Where(x => x.Id == projectId)
            .Include(x => x.ProjectUsers)
            .OrderBy(x => x.Name)
            .SelectMany(x => x.ProjectUsers)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Projects
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .Include(x => x.Creator)
            .Include(x => x.Client)
            .Include(x => x.ProjectUsers)
            .OrderBy(x => x.Name)
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
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAllByClient(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Projects
            .AsNoTracking()
            .Where(x => x.ClientId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAllByCreator(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Projects
            .AsNoTracking()
            .Where(x => x.CreatorId == userId)
            .OrderBy(x => x.Name)
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
            context.Projects.Add(project);
        }
        else
        {
            context.Entry(existingEntry).CurrentValues.SetValues(project);
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