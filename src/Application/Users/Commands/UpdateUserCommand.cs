using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Users.Exceptions;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record UpdateUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserQueries _userQueries;

    public UpdateUserCommandHandler(UserManager<User> userManager, IUserQueries userQueries)
    {
        _userManager = userManager;
        _userQueries = userQueries;
    }

    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userQueries.GetById(
            request.UserId,
            cancellationToken);

        return await existingUser.Match(
            async ue =>
            {
                var existingUserWithUserName = await _userQueries.SearchByUserName(
                    request.UserName,
                    cancellationToken);
                return await existingUserWithUserName.Match(
                    un => Task.FromResult<Result<User, UserException>>(
                        new UserWithNameAlreadyExistsException(un.Id, un.UserName)),
                    async () =>
                    {
                        var existingUserWithEmail = await _userQueries.SearchByEmail(
                            request.Email,
                            cancellationToken);
                        return await existingUserWithEmail.Match<Task<Result<User, UserException>>>(
                            ue => Task.FromResult<Result<User, UserException>>(
                                new UserWithEmailAlreadyExistsException(ue.Id, ue.Email)),
                            async () => await UpdateEntity(request, ue));
                    });
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(request.UserId))
        );
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        UpdateUserCommand request,
        User existingUser)
    {
        try
        {
            existingUser.Email = request.Email;
            existingUser.UserName = request.UserName;
            existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser, request.Password);
            existingUser.SecurityStamp = Guid.NewGuid().ToString();

            var result = await _userManager.UpdateAsync(existingUser);
            return Result<User, UserException>.FromIdentityResult<User, UserException>(result, existingUser,
                e => new UserUnknownException(request.UserId, new Exception(e.ToString())));
        }
        catch (Exception exception)
        {
            return new UserUnknownException(Guid.Empty, exception);
        }
    }
}