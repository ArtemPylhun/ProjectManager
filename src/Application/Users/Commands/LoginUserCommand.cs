using Application.Common;
using Application.Common.Interfaces;
using Application.Users.Exceptions;
using Domain.Models.Roles;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public class LoginUserCommand : IRequest<Result<string, UserException>>
{
    public string EmailOrUsername { get; init; }
    public required string Password { get; init; }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string, UserException>>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserCommandHandler(UserManager<User> userManager,
        SignInManager<User> signInManager, 
        RoleManager<Role> roleManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _roleManager = roleManager; 
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string, UserException>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.EmailOrUsername);
        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(request.EmailOrUsername);
        }

        if (user == null)
        {
            return await Task.FromResult<Result<string, UserException>>(new UserNotFoundException(Guid.Empty));
        }
        
        
        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
        if (!result.Succeeded)
        {
            return await Task.FromResult<Result<string, UserException>>(new InvalidCredentialsException());
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            return await Task.FromResult<Result<string, UserException>>(new UserRolesNotFoundException(Guid.Empty));
        }
        
        var token = _jwtProvider.GenerateToken(user, roles.ToList());

        if (!String.IsNullOrEmpty(token))
        {
            return await Task.FromResult(Result<string, UserException>.Success(token));
        }
        
        return await Task.FromResult<Result<string, UserException>>(new InvalidTokenException());
    }
}