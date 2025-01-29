using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Projects.Exceptions;
using Domain.Models.Projects;
using MediatR;

namespace Application.Projects.Commands;

public record DeleteProjectCommand : IRequest<Result<Project, ProjectException>>
{
    public Guid Id { get; init; }
}

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Result<Project, ProjectException>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectQueries _projectQueries;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository, IProjectQueries projectQueries)
    {
        _projectRepository = projectRepository;
        _projectQueries = projectQueries;
    }

    public async Task<Result<Project, ProjectException>> Handle(DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(request.Id);
        var project = await _projectQueries.GetById(projectId, cancellationToken);
        return await project.Match(
            async p => await DeleteEntity(p, cancellationToken),
            async () => await Task.FromResult(
                Result<Project, ProjectException>.Failure(new ProjectNotFoundException(projectId)))
            );
    }
    
    private async Task<Result<Project, ProjectException>> DeleteEntity(
        Project entity,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _projectRepository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ProjectUnknownException(ProjectId.Empty(), exception);
        }
    }
}