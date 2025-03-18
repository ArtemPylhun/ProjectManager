using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Users.Exceptions;
using Domain.Models.Roles;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<string, UserException>>
{
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
}

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result<string, UserException>>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IEmailViewRenderer _emailViewRenderer;
    private readonly IJwtProvider _jwtProvider;

    public CreateUserCommandHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IEmailService emailService,
        IEmailViewRenderer emailViewRenderer, 
        IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _emailViewRenderer = emailViewRenderer;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string, UserException>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUserWithUserName = await _userManager.FindByNameAsync(
            request.UserName);

        if (existingUserWithUserName != null)
        {
            return await Task.FromResult<Result<string, UserException>>(
                new UserWithNameAlreadyExistsException(existingUserWithUserName.Id, existingUserWithUserName.UserName));
        }
        
        var existingUserWithEmail = await _userManager.FindByEmailAsync(
            request.Email);

        if (existingUserWithEmail != null)
        {
            return await Task.FromResult<Result<string, UserException>>(
                new UserWithEmailAlreadyExistsException(existingUserWithEmail.Id, existingUserWithEmail.Email));

        }
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = false
        };
        var entity = await _userManager.CreateAsync(user, request.Password);
        var roleName = "User";
        var userRole = await _roleManager.FindByNameAsync(roleName);
        if (userRole == null)
        {
            await _roleManager.CreateAsync(new Role { Name = roleName });
        }
        await _userManager.AddToRoleAsync(user, roleName);
            
        // Generate email verification token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        user.EmailVerificationToken = token;
        user.EmailVerificationTokenExpiration = DateTime.UtcNow.AddHours(24);

        // Update the user with the verification token
        await _userManager.UpdateAsync(user);

        // Send verification email using EmailViewRenderer
        var verificationLink = $"https://localhost:44347/users/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        var model = (UserName: user.UserName, VerificationLink: verificationLink);
        var subject = "Verify Your Email Address";
        var htmlBody = _emailViewRenderer.RenderView("RegistrationEmailSent", model, user.Email, subject);

        _emailService.SendEmail(user.Email, subject, htmlBody, isHtml: true);

        // Return JWT token, but user must verify email before logging in
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            return await Task.FromResult<Result<string, UserException>>(new UserRolesNotFoundException(Guid.Empty));
        }
        
        return _jwtProvider.GenerateToken(user, roles.ToList());
    }
    
}