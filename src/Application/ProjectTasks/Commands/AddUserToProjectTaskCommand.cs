using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProjectTasks.Exceptions;
using Domain.Models.ProjectTasks;
using Domain.Models.Users;
using Domain.Models.UsersTasks;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.ProjectTasks.Commands;

public record AddUserToProjectTaskCommand : IRequest<Result<UserTask, UserTaskException>>
{
    public Guid ProjectTaskId { get; init; }
    public Guid UserId { get; init; }
}

public class AddUserToProjectTaskCommandHandler : IRequestHandler<AddUserToProjectTaskCommand,
    Result<UserTask, UserTaskException>>
{
    private readonly IUserTaskQueries _userTaskQueries;
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly IProjectTaskQueries _projectTaskQueries;
    private readonly UserManager<User> _userManager;

    public AddUserToProjectTaskCommandHandler(IUserTaskQueries userTaskQueries, IUserTaskRepository userTaskRepository,
        IProjectTaskQueries projectTaskQueries, UserManager<User> userManager)
    {
        _userTaskQueries = userTaskQueries;
        _userTaskRepository = userTaskRepository;
        _projectTaskQueries = projectTaskQueries;
        _userManager = userManager;
    }


    public async Task<Result<UserTask, UserTaskException>> Handle(AddUserToProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        var projectTaskId = new ProjectTaskId(request.ProjectTaskId);
        var projectTask = await _projectTaskQueries.GetById(projectTaskId, cancellationToken);
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return await Task.FromResult(
                Result<UserTask, UserTaskException>.Failure(
                    new UserTaskUserNotFoundException(UserTaskId.Empty(), request.UserId)));
        }

        return await projectTask.Match(
            async p =>
            {
                var existingUserTask =
                    await _userTaskQueries.GetByProjectTaskAndUserIds(p.Id, user.Id, cancellationToken);
                return await existingUserTask.Match(
                    async pu => await Task.FromResult(
                        Result<UserTask, UserTaskException>.Failure(
                            new UserTaskAlreadyExistsException(pu.Id))),
                    async () =>
                    {
                        UserTask newUserTask = UserTask.New(UserTaskId.New(), p.Id, user.Id);
                        return await CreateEntity(newUserTask, cancellationToken);
                    });
            },
            async () => await Task.FromResult(
                Result<UserTask, UserTaskException>.Failure(
                    new UserTaskProjectTaskNotFoundException(UserTaskId.Empty(), projectTaskId))));
    }

    private async Task<Result<UserTask, UserTaskException>> CreateEntity(
        UserTask entity,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userTaskRepository.Add(entity, cancellationToken);

            return result;
        }
        catch (Exception exception)
        {
            return new UserTaskUnknownException(entity.Id, exception);
        }
    }
}