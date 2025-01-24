using Domain.Models.ProjectTasks;
using Domain.Models.UsersTasks;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserTaskQueries
{
    Task<IReadOnlyList<UserTask>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<UserTask>> GetAllByProjectTaskId(ProjectTaskId projectTaskId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserTask>> GetAllByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Option<UserTask>> GetById(UserTaskId id, CancellationToken cancellationToken);
    Task<Option<UserTask>> GetByProjectTaskAndUserIds(ProjectTaskId projectTaskId, Guid userId, CancellationToken cancellationToken);
}