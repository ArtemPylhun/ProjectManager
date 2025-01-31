using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Projects.Exceptions;
using Domain.Models.Projects;
using Domain.Models.ProjectUsers;
using Domain.Models.Roles;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Projects.Commands;

public record AddUserToProjectCommand: IRequest<Result<ProjectUser, ProjectUserException>>
{
    public Guid ProjectId { get; init; }
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

public class
    AddUserToProjectCommandHandler : IRequestHandler<AddUserToProjectCommand, Result<ProjectUser, ProjectUserException>>
{
    private readonly IProjectUserQueries _projectUserQueries;
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IProjectQueries _projectQueries;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public AddUserToProjectCommandHandler(IProjectUserQueries projectUserQueries,
        IProjectUserRepository projectUserRepository, IProjectQueries projectQueries, UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _projectUserQueries = projectUserQueries;
        _projectUserRepository = projectUserRepository;
        _projectQueries = projectQueries;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<ProjectUser, ProjectUserException>> Handle(AddUserToProjectCommand request,
        CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(request.ProjectId);
        var project = await _projectQueries.GetById(projectId, cancellationToken);
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        if (user == null)
        {
            return await Task.FromResult(
                Result<ProjectUser, ProjectUserException>.Failure(
                    new ProjectUserUserNotFoundException(ProjectUserId.Empty(), request.UserId)));
        }

        if (role == null)
        {
            return await Task.FromResult(
                Result<ProjectUser, ProjectUserException>.Failure(
                    new ProjectUserRoleNotFoundException(ProjectUserId.Empty(), request.RoleId)));
        }

        return await project.Match(
            async p =>
            {
                var existingProjectUser =
                    await _projectUserQueries.GetByProjectAndUserIds(p.Id, user.Id, cancellationToken);
                return await existingProjectUser.Match(
                    async pu => await Task.FromResult(
                        Result<ProjectUser, ProjectUserException>.Failure(
                            new ProjectUserAlreadyExistsException(pu.Id))),
                    async () =>
                    {
                        ProjectUser newProjectUser = ProjectUser.New(ProjectUserId.New(), p.Id, user.Id, role.Id);
                        return await CreateEntity(newProjectUser, cancellationToken);
                    });
            },
            async () => await Task.FromResult(
                Result<ProjectUser, ProjectUserException>.Failure(
                    new ProjectUserProjectNotFoundException(ProjectUserId.Empty(), projectId))));
    }
    
    private async Task<Result<ProjectUser, ProjectUserException>> CreateEntity(
        ProjectUser entity,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _projectUserRepository.Add(entity, cancellationToken);
            
            return result;
        }
        catch (Exception exception)
        {
            return new ProjectUserUnknownException(entity.Id, exception);
        }
    }
}
    