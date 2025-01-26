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
    private readonly IUserQueries _userQueries;

    public CreateUserCommandHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IUserQueries userQueries)
    {
        _userManager = userManager;
        _userQueries = userQueries;
        _roleManager = roleManager;
    }

    public async Task<Result<User, UserException>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUserWithUserName = await _userQueries.SearchByUserName(
            request.UserName,
            cancellationToken);


        return await existingUserWithUserName.Match(
            un => Task.FromResult<Result<User, UserException>>(new UserWithNameAlreadyExistsException(un.Id, un.UserName)),
            async () =>
            {
                var existingUserWithEmail = await _userQueries.SearchByEmail(
                    request.Email,
                    cancellationToken);
                return await existingUserWithEmail.Match<Task<Result<User, UserException>>>(
                    ue => Task.FromResult<Result<User, UserException>>(
                        new UserWithEmailAlreadyExistsException(ue.Id, ue.Email)),
                    async () => await CreateEntity(request, cancellationToken));
            });
    }
    
    private async Task<Result<User, UserException>> CreateEntity(
        CreateUserCommand request,
        CancellationToken cancellationToken)
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
                await _roleManager.CreateAsync(new Role { Name = "User"});
            }
            await _userManager.AddToRoleAsync(user, "User");
            
            return Result<User,UserException>.FromIdentityResult<User, UserException>(entity, user, e => new UserUnknownException(user.Id, new Exception("User creation failed")));
        }
        catch (Exception exception)
        {
            return new UserUnknownException(Guid.Empty, exception);
        }
    }
    
}