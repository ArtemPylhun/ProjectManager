using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProjectTasks.Exceptions;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.ProjectTasks.Commands;

public record CreateProjectTaskCommand : IRequest<Result<ProjectTask, ProjectTaskException>>
{
    public Guid ProjectId { get; init; }
    public Guid CreatorId { get; init; }
    public string Name { get; init; }
    public int EstimatedTime { get; init; }
    public string Description { get; init; }
}

public class
    CreateProjectCommandHandler : IRequestHandler<CreateProjectTaskCommand, Result<ProjectTask, ProjectTaskException>>
{
    private readonly IProjectTaskQueries _projectTaskQueries;
    private readonly IProjectTaskRepository _projectTaskRepository;
    private readonly IProjectQueries _projectQueries;
    private readonly UserManager<User> _userManager;

    public CreateProjectCommandHandler(IProjectTaskQueries projectTaskQueries,
        IProjectTaskRepository projectTaskRepository, IProjectQueries projectQueries, UserManager<User> userManager)
    {
        _projectTaskQueries = projectTaskQueries;
        _projectTaskRepository = projectTaskRepository;
        _projectQueries = projectQueries;
        _userManager = userManager;
    }

    public async Task<Result<ProjectTask, ProjectTaskException>> Handle(CreateProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        var creator = _userManager.FindByIdAsync(request.CreatorId.ToString()).Result;
        if (creator == null)
        {
            return await Task.FromResult(
                Result<ProjectTask, ProjectTaskException>.Failure(new CreatorNotFoundException(request.CreatorId)));
        }
        
        var projectId = new ProjectId(request.ProjectId);
        var project = await _projectQueries.GetById(projectId, cancellationToken);

        return await project.Match(
            async p =>
            {
                var existingProjectTasks = await _projectTaskQueries.GetAllByProjectId(p.Id, cancellationToken);

                if (existingProjectTasks.Any(pt => pt.Name == request.Name))
                {
                    return await Task.FromResult(Result<ProjectTask, ProjectTaskException>.Failure(
                        new ProjectTaskAlreadyExistsException(ProjectTaskId.Empty(), p.Name, request.Name)));
                }

                ProjectTask newProjectTask = ProjectTask.New(ProjectTaskId.New(), p.Id, request.Name,
                    request.EstimatedTime, request.Description, DateTime.UtcNow, request.CreatorId);
                return await CreateEntity(newProjectTask, cancellationToken);
            },
            () => Task.FromResult(
                Result<ProjectTask, ProjectTaskException>.Failure(
                    new ProjectForTaskNotFoundException(ProjectTaskId.Empty(), projectId))));
    }

    private async Task<Result<ProjectTask, ProjectTaskException>> CreateEntity(
        ProjectTask entity,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _projectTaskRepository.Add(entity, cancellationToken);
            return result;
        }
        catch (Exception exception)
        {
            return new ProjectTaskUnknownException(ProjectTaskId.Empty(), exception);
        }
    }
}