using Domain.Models.UsersTasks;

namespace Application.Common.Interfaces.Repositories;

public interface IUserTaskRepository
{
    Task<UserTask> Add(UserTask userTask, CancellationToken cancellationToken);
    Task<UserTask> Update(UserTask userTask, CancellationToken cancellationToken);
    Task<UserTask> Delete(UserTask userTask, CancellationToken cancellationToken);
}