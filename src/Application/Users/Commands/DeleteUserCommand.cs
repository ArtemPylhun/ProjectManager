using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Users.Exceptions;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record DeleteUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
}

public class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Result<User, UserException>>
{
    private readonly UserManager<User> _userManager;
    private readonly IProjectQueries _projectQueries;

    public DeleteUserCommandHandler(UserManager<User> userManager, IProjectQueries projectQueries)
    {
        _userManager = userManager;
        _projectQueries = projectQueries;
    }

    public async Task<Result<User, UserException>> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (existingUser is null)
        {
            return await Task.FromResult<Result<User, UserException>>(new UserNotFoundException());
        }

        var projectsWithCreator = await _projectQueries.GetAllByCreator(existingUser.Id, cancellationToken);
        var projectsWithClient = await _projectQueries.GetAllByClient(existingUser.Id, cancellationToken);

        if (projectsWithClient.Count > 0 || projectsWithCreator.Count > 0)
        {
            return await Task.FromResult<Result<User, UserException>>(new UserAlreadyUsedInProject(request.UserId));
        }
        
        var result = await _userManager.DeleteAsync(existingUser);
        return Result<User, UserException>.FromIdentityResult<User, UserException>(result, existingUser,
                     e => new UserUnknownException(request.UserId, new Exception(e.ToString())));
    }
}