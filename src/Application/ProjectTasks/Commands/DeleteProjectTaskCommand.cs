using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProjectTasks.Exceptions;
using Domain.Models.ProjectTasks;
using MediatR;

namespace Application.ProjectTasks.Commands;

public record DeleteProjectTaskCommand: IRequest<Result<ProjectTask, ProjectTaskException>>
{
    public Guid ProjectTaskId { get; init; }
}

public class DeleteProjectTaskCommandHandler : IRequestHandler<DeleteProjectTaskCommand, Result<ProjectTask, ProjectTaskException>>
{
    private readonly IProjectTaskRepository _projectTaskRepository;
    private readonly IProjectTaskQueries _projectTaskQueries;

    public DeleteProjectTaskCommandHandler(IProjectTaskRepository projectTaskRepository, IProjectTaskQueries projectTaskQueries)
    {
        _projectTaskRepository = projectTaskRepository;
        _projectTaskQueries = projectTaskQueries;
    }

    public async Task<Result<ProjectTask, ProjectTaskException>> Handle(DeleteProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        var projectTaskId = new ProjectTaskId(request.ProjectTaskId);
        var projectTask = await _projectTaskQueries.GetById(projectTaskId, cancellationToken);
        return await projectTask.Match(
            async p => await DeleteEntity(p, cancellationToken),
            async () => await Task.FromResult(
                Result<ProjectTask, ProjectTaskException>.Failure(new ProjectTaskNotFoundException(projectTaskId)))
        );
    }
    
    private async Task<Result<ProjectTask, ProjectTaskException>> DeleteEntity(
        ProjectTask entity,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _projectTaskRepository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ProjectTaskUnknownException(entity.Id, exception);
        }
    }
}