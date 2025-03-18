using Application.Common;
using Application.Users.Exceptions;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public class VerifyEmailCommand: IRequest<Result<(bool success, string? userName, string? email), UserException>>
{
    public required Guid UserId { get; init; }
    public required string Token { get; init; }
}

public class VerifyEmailCommandHandler(
    UserManager<User> userManager) : IRequestHandler<VerifyEmailCommand, Result<(bool success, string? userName, string? email), UserException>>
{
    public async Task<Result<(bool success, string? userName, string? email), UserException>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return await Task.FromResult<Result<(bool success, string? userName, string? email), UserException>>(
                new UserNotFoundException());
        }

        if (user.EmailVerificationTokenExpiration < DateTime.UtcNow)
        {
            return await Task.FromResult<Result<(bool success, string? userName, string? email), UserException>>(
                new EmailVerificationTokenExpiredException(request.UserId));
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
           
            return await Task.FromResult<Result<(bool success, string? userName, string? email), UserException>>(
                new InvalidVerificationTokenException(request.UserId));
            
        }

        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiration = null;
        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);

        return (success: true, userName: user.UserName, email: user.Email);
    }
}