using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Projects.Exceptions;
using Domain.Models.ProjectUsers;
using MediatR;

namespace Application.Projects.Commands;

public record RemoveUserFromProjectCommand : IRequest<Result<ProjectUser, ProjectUserException>>
{
    public Guid ProjectUserId { get; init; }
}

public class
    RemoveUserFromProjectCommandHandler : IRequestHandler<RemoveUserFromProjectCommand,
    Result<ProjectUser, ProjectUserException>>
{
    private readonly IProjectUserQueries _projectUserQueries;
    private readonly IProjectUserRepository _projectUserRepository;

    public RemoveUserFromProjectCommandHandler(IProjectUserQueries projectUserQueries,
        IProjectUserRepository projectUserRepository)
    {
        _projectUserQueries = projectUserQueries;
        _projectUserRepository = projectUserRepository;
    }

    public async Task<Result<ProjectUser, ProjectUserException>> Handle(RemoveUserFromProjectCommand request,
        CancellationToken cancellationToken)
    {
        var projectUserId = new ProjectUserId(request.ProjectUserId);
        var projectUser = await _projectUserQueries.GetById(projectUserId, cancellationToken);

        return await projectUser.Match(
            async p => await DeleteEntity(p, cancellationToken),
            async () => await Task.FromResult(
                Result<ProjectUser, ProjectUserException>.Failure(new ProjectUserNotFoundException(projectUserId)))
        );
    }

    private async Task<Result<ProjectUser, ProjectUserException>> DeleteEntity(
            ProjectUser entity,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _projectUserRepository.Delete(entity, cancellationToken);
            }
            catch (Exception exception)
            {
                return new ProjectUserUnknownException(entity.Id, exception);
            }
        }
    }