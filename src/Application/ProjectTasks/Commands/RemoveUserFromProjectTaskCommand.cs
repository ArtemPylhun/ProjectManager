using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProjectTasks.Exceptions;
using Domain.Models.UsersTasks;
using MediatR;

namespace Application.ProjectTasks.Commands;

public record RemoveUserFromProjectTaskCommand : IRequest<Result<UserTask, UserTaskException>>
{
    public Guid UserTaskId { get; init; }
}

public class
    RemoveUserFromProjectTaskCommandHandler : IRequestHandler<RemoveUserFromProjectTaskCommand,
    Result<UserTask, UserTaskException>>
{
    private readonly IUserTaskQueries _userTaskQueries;
    private readonly IUserTaskRepository _userTaskRepository;

    public RemoveUserFromProjectTaskCommandHandler(IUserTaskQueries userTaskQueries,
        IUserTaskRepository userTaskRepository)
    {
        _userTaskQueries = userTaskQueries;
        _userTaskRepository = userTaskRepository;
    }

    public async Task<Result<UserTask, UserTaskException>> Handle(RemoveUserFromProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        var userTaskId = new UserTaskId(request.UserTaskId);
        var userTask = await _userTaskQueries.GetById(userTaskId, cancellationToken);

        return await userTask.Match(
            async p => await DeleteEntity(p, cancellationToken),
            async () => await Task.FromResult(
                Result<UserTask, UserTaskException>.Failure(new UserTaskNotFoundException(userTaskId)))
        );
    }

    private async Task<Result<UserTask, UserTaskException>> DeleteEntity(
        UserTask entity,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _userTaskRepository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserTaskUnknownException(entity.Id, exception);
        }
    }
}