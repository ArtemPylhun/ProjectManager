using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProjectTasks.Exceptions;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using MediatR;

namespace Application.ProjectTasks.Commands;

public record UpdateProjectTaskCommand: IRequest<Result<ProjectTask, ProjectTaskException>>
{
    public Guid ProjectTaskId { get; init; }
    public Guid ProjectId { get; init; }
    public string Name { get; init; }
    public int EstimatedTime { get; init; }
    public string Description { get; init; }
    public ProjectTask.ProjectTaskStatuses Status { get; init; }
}

public class
    UpdateProjectCommandHandler : IRequestHandler<UpdateProjectTaskCommand, Result<ProjectTask, ProjectTaskException>>
{
    private readonly IProjectTaskQueries _projectTaskQueries;
    private readonly IProjectTaskRepository _projectTaskRepository;
    private readonly IProjectQueries _projectQueries;

    public UpdateProjectCommandHandler(IProjectTaskQueries projectTaskQueries, IProjectTaskRepository projectTaskRepository, IProjectQueries projectQueries)
    {
        _projectTaskQueries = projectTaskQueries;
        _projectTaskRepository = projectTaskRepository;
        _projectQueries = projectQueries;
    }


    public async Task<Result<ProjectTask, ProjectTaskException>> Handle(UpdateProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var projectTaskId = new ProjectTaskId(request.ProjectTaskId);
        var projectTask = await _projectTaskQueries.GetById(projectTaskId, cancellationToken);

        return await projectTask.Match(
            async pt =>
            {
                var projectId = new ProjectId(request.ProjectId);
                var project = await _projectQueries.GetById(projectId, cancellationToken);
                return await project.Match(
                    async p =>
                    {
                        var existingProjectTasks = await _projectTaskQueries.GetAllByProjectId(p.Id, cancellationToken);
                        if (existingProjectTasks.Where(x => x.Id.Value != pt.Id.Value).Any(task => task.Name == request.Name))
                        {
                            return await Task.FromResult(Result<ProjectTask, ProjectTaskException>.Failure(
                                new ProjectTaskAlreadyExistsException(ProjectTaskId.Empty(), p.Name, request.Name)));
                        }

                        return await UpdateEntity(pt, projectId, request.Name, request.EstimatedTime, request.Description, request.Status, cancellationToken);
                    },
                    async () => await Task.FromResult(
                        Result<ProjectTask, ProjectTaskException>.Failure(
                            new ProjectForTaskNotFoundException(projectTaskId, projectId))));

            },
            async () => await Task.FromResult(
                Result<ProjectTask, ProjectTaskException>.Failure(new ProjectTaskNotFoundException(projectTaskId))));
    }
    
    private async Task<Result<ProjectTask, ProjectTaskException>> UpdateEntity(
        ProjectTask entity,
        ProjectId projectId,
        string name,
        int estimatedTime,
        string description,
        ProjectTask.ProjectTaskStatuses status,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(projectId, name, estimatedTime, description, status);
            var result = await _projectTaskRepository.Update(entity, cancellationToken);
            return result;
        }
        catch (Exception exception)
        {
            return new ProjectTaskUnknownException(entity.Id, exception);
        }
    }
}