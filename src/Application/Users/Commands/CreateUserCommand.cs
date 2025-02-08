using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Users.Exceptions;
using Domain.Models.Roles;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
}

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public CreateUserCommandHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<User, UserException>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUserWithUserName = await _userManager.FindByNameAsync(
            request.UserName);

        if (existingUserWithUserName != null)
        {
            return await Task.FromResult<Result<User, UserException>>(
                new UserWithNameAlreadyExistsException(existingUserWithUserName.Id, existingUserWithUserName.UserName));
        }
        
        var existingUserWithEmail = await _userManager.FindByEmailAsync(
            request.Email);

        if (existingUserWithEmail != null)
        {
            return await Task.FromResult<Result<User, UserException>>(
                new UserWithEmailAlreadyExistsException(existingUserWithEmail.Id, existingUserWithEmail.Email));

        }

        return await CreateEntity(request);
    }

    private async Task<Result<User, UserException>> CreateEntity(
        CreateUserCommand request)
    {
        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email
            };
            var entity = await _userManager.CreateAsync(user, request.Password);
            var userRole = await _roleManager.FindByNameAsync("User");
            if (userRole == null)
            {
                await _roleManager.CreateAsync(new Role { Name = "User" });
            }
            await _userManager.AddToRoleAsync(user, "User");

            return Result<User, UserException>.FromIdentityResult<User, UserException>(entity, user,
                e => new UserUnknownException(user.Id, new Exception("User creation failed")));
        }
        catch (Exception exception)
        {
            return new UserUnknownException(Guid.Empty, exception);
        }
    }
}