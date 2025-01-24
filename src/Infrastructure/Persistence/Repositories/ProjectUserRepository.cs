using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Projects;
using Domain.Models.ProjectUsers;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ProjectUserRepository(ApplicationDbContext context) : IProjectUserRepository, IProjectUserQueries
{
    public async Task<ProjectUser> Add(ProjectUser projectUser, CancellationToken cancellationToken)
    {
        await context.ProjectUsers.AddAsync(projectUser, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return projectUser;
    }

    public async Task<ProjectUser> Update(ProjectUser projectUser, CancellationToken cancellationToken)
    {
        context.ProjectUsers.Update(projectUser);

        await context.SaveChangesAsync(cancellationToken);

        return projectUser;
    }

    public async Task<ProjectUser> Delete(ProjectUser projectUser, CancellationToken cancellationToken)
    {
        context.ProjectUsers.Remove(projectUser);

        await context.SaveChangesAsync(cancellationToken);

        return projectUser;
    }

    public async Task<IReadOnlyList<ProjectUser>> GetAll(CancellationToken cancellationToken)
    {
        return await context.ProjectUsers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectUser>> GetAllByProjectId(ProjectId projectId, CancellationToken cancellationToken)
    {
        return await context.ProjectUsers
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectUser>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await context.ProjectUsers
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ProjectUser>> GetById(ProjectUserId id, CancellationToken cancellationToken)
    {
        var entity =  await context.ProjectUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<ProjectUser>() : Option.Some(entity);

    }

    public async Task<Option<ProjectUser>> GetByProjectAndUserIds(ProjectId projectId, Guid userId, CancellationToken cancellationToken)
    {
        var entity =  await context.ProjectUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == userId, cancellationToken);

        return entity == null ? Option.None<ProjectUser>() : Option.Some(entity);
    }
}