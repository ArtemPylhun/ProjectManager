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
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    private readonly UserManager<User> _userManager;

    public UpdateUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByIdAsync(
            request.UserId.ToString());

        if (existingUser == null)
        {
            return await Task.FromResult<Result<User, UserException>>(new UserNotFoundException(request.UserId));
        }
        var existingUserWithUserName = await _userManager.FindByNameAsync(
            request.UserName);

        if (existingUserWithUserName != null && existingUserWithUserName.Id != existingUser.Id)
        {
            return await Task.FromResult<Result<User, UserException>>(
                new UserWithNameAlreadyExistsException(existingUserWithUserName.Id, 
                    existingUserWithUserName.UserName));
        }
        
        var existingUserWithEmail = await _userManager.FindByEmailAsync(
            request.Email);

        if (existingUserWithEmail != null && existingUserWithEmail.Id != existingUser.Id)
        {
            return await Task.FromResult<Result<User, UserException>>(
                new UserWithEmailAlreadyExistsException(existingUserWithEmail.Id, 
                    existingUserWithEmail.Email));
        }

        return await UpdateEntity(request, existingUser);
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        UpdateUserCommand request,
        User existingUser)
    {
        try
        {
            existingUser.Email = request.Email;
            existingUser.UserName = request.UserName;
            //existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser, request.Password);
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