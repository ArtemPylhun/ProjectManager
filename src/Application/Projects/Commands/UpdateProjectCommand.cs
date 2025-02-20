using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Projects.Exceptions;
using Domain.Models.Projects;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Projects.Commands;

public record UpdateProjectCommand : IRequest<Result<Project, ProjectException>>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string ColorHex { get; init; }
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result<Project, ProjectException>>
{
    private readonly UserManager<User> _userManager;
    private readonly IProjectQueries _projectQueries;
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectCommandHandler(UserManager<User> userManager, IProjectQueries projectQueries,
        IProjectRepository projectRepository)
    {
        _userManager = userManager;
        _projectQueries = projectQueries;
        _projectRepository = projectRepository;
    }

    public async Task<Result<Project, ProjectException>> Handle(UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {

        var projectId = new ProjectId(request.Id);
        var existingProject = await _projectQueries.GetById(projectId, cancellationToken);

        return await existingProject.Match(
            async p => 
            {
                var existingProjectWithSameName = await _projectQueries.GetByName(request.Name, cancellationToken);
                return await existingProjectWithSameName.Match(
                    async foundProject =>
                        foundProject.Id != p.Id
                            ? Result<Project, ProjectException>.Failure(new ProjectAlreadyExistsException(foundProject.Id, foundProject.Name))
                            : await UpdateEntity(p, request.Name, request.Description, request.ColorHex, cancellationToken),
                    async () => await UpdateEntity(p, request.Name, request.Description, request.ColorHex, cancellationToken)
                );
            },
            async () => Result<Project, ProjectException>.Failure(new ProjectNotFoundException(projectId))
        );
    }

    private async Task<Result<Project, ProjectException>> UpdateEntity(
        Project entity,
        string name,
        string description,
        string colorHex,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name,description,colorHex);
            return await _projectRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ProjectUnknownException(entity.Id, exception);
        }
    }
}