using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Projects.Exceptions;
using Domain.Models.Projects;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Projects.Commands;

public record CreateProjectCommand : IRequest<Result<Project, ProjectException>>
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string ColorHex { get; init; }
    public Guid ClientId { get; init; }
    public Guid CreatorId { get; init; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<Project, ProjectException>>
{
    private readonly UserManager<User> _userManager;
    private readonly IProjectQueries _projectQueries;
    private readonly IProjectRepository _projectRepository;

    public CreateProjectCommandHandler(UserManager<User> userManager, IProjectQueries projectQueries,
        IProjectRepository projectRepository)
    {
        _userManager = userManager;
        _projectQueries = projectQueries;
        _projectRepository = projectRepository;
    }

    public async Task<Result<Project, ProjectException>> Handle(CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var creator = _userManager.FindByIdAsync(request.CreatorId.ToString()).Result;
        if (creator == null)
        {
            return await Task.FromResult(
                Result<Project, ProjectException>.Failure(new CreatorNotFoundException(request.CreatorId)));
        }

        var client = _userManager.FindByIdAsync(request.ClientId.ToString()).Result;
        if (client == null)
        {
            return await Task.FromResult(
                Result<Project, ProjectException>.Failure(new ClientNotFoundException(request.ClientId)));
        }

        var existingProject = await _projectQueries.GetByName(request.Name, cancellationToken);

        return await existingProject.Match(
            async p => await Task.FromResult(
                Result<Project, ProjectException>.Failure(new ProjectAlreadyExistsException(p.Id, p.Name))),
            async () =>
            {
                Project newProject = Project.New(ProjectId.New(), request.Name, request.Description, DateTime.UtcNow,
                    request.CreatorId, request.ColorHex, request.ClientId);
                return await CreateEntity(newProject, cancellationToken);
            });
    }

    private async Task<Result<Project, ProjectException>> CreateEntity(
        Project entity,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _projectRepository.Add(entity, cancellationToken);
            
            return result;
        }
        catch (Exception exception)
        {
            return new ProjectUnknownException(ProjectId.Empty(), exception);
        }
    }
}

//TODO: create UserProjectRole too