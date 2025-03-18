using Application.Common;
using Application.Common.Interfaces.Services;
using Application.Users.Exceptions;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public class ResendEmailVerificationCommand : IRequest<Result<bool, UserException>>
{
    public required string Email { get; init; }
}

public class ResendVerificationEmailCommandHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    IEmailViewRenderer emailViewRenderer) : IRequestHandler<ResendEmailVerificationCommand, Result<bool, UserException>>
{
    public async Task<Result<bool, UserException>> Handle(ResendEmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return await Task.FromResult<Result<bool, UserException>>(new UserNotFoundException());
        }

        if (user.EmailConfirmed)
        {
            return true;
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        user.EmailVerificationToken = token;
        user.EmailVerificationTokenExpiration = DateTime.UtcNow.AddHours(24);

        await userManager.UpdateAsync(user);

        var verificationLink =
            $"https://localhost:44347/users/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        var model = (UserName: user.UserName, VerificationLink: verificationLink);
        var subject = "Verify Your Email Address";
        var htmlBody = emailViewRenderer.RenderView("ResendVerificationEmail", model, user.Email, subject);

        emailService.SendEmail(user.Email, subject, htmlBody, isHtml: true);

        return true;
    }
}