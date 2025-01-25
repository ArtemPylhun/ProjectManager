using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.ProjectTasks;
using Domain.Models.UsersTasks;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserTaskRepository(ApplicationDbContext context) : IUserTaskRepository, IUserTaskQueries
{
    public async Task<UserTask> Add(UserTask userTask, CancellationToken cancellationToken)
    {
        await context.UserTasks.AddAsync(userTask, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return userTask;
    }

    public async Task<UserTask> Update(UserTask userTask, CancellationToken cancellationToken)
    {
        context.UserTasks.Update(userTask);

        await context.SaveChangesAsync(cancellationToken);

        return userTask;
    }

    public async Task<UserTask> Delete(UserTask userTask, CancellationToken cancellationToken)
    {
        context.UserTasks.Remove(userTask);

        await context.SaveChangesAsync(cancellationToken);

        return userTask;
    }

    public async Task<IReadOnlyList<UserTask>> GetAll(CancellationToken cancellationToken)
    {
        return await context.UserTasks
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserTask>> GetAllByProjectTaskId(ProjectTaskId projectTaskId,
        CancellationToken cancellationToken)
    {
        return await context.UserTasks
            .AsNoTracking()
            .Where(x => x.ProjectTaskId == projectTaskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserTask>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await context.UserTasks
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<UserTask>> GetById(UserTaskId id, CancellationToken cancellationToken)
    {
        var entity = await context.UserTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<UserTask>() : Option.Some(entity);
    }

    public async Task<Option<UserTask>> GetByProjectTaskAndUserIds(ProjectTaskId projectTaskId, Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = await context.UserTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProjectTaskId == projectTaskId && x.UserId == userId, cancellationToken);

        return entity == null ? Option.None<UserTask>() : Option.Some(entity);
    }
}