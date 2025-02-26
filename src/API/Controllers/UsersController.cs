using System.Security.Claims;
using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces;
using Application.Users.Commands;
using Domain.Models.Roles;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("users")]
[ApiController]
public class UsersController(ISender sender, UserManager<User> userManager, RoleManager<Role> roleManager, IJwtProvider _jwtProvider) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userManager.Users.ToListAsync(cancellationToken);
        return entities.Select(UserDto.FromDomainModel).ToList();
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserWithRolesDto>> GetByIdWithRoles([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound();

        var roles = await userManager.GetRolesAsync(user);
        return UserWithRolesDto.FromDomainModel(user, roles);
    }

    [HttpGet("get-all-with-roles")]
    public async Task<ActionResult<IReadOnlyList<UserWithRolesDto>>> GetAllWithRoles(
        CancellationToken cancellationToken)
    {
        var entities = userManager.Users.ToList();
        var result = new List<UserWithRolesDto>();
        foreach (var user in entities)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(UserWithRolesDto.FromDomainModel(user, roles));
        }

        return result.ToList();
    }
    
    [HttpPost("login")]
    public async Task<ActionResult> LoginUser([FromBody] UserLoginDto request, CancellationToken cancellationToken)
    {
        var input = new LoginUserCommand
        {
            EmailOrUsername = request.EmailOrUsername,
            Password = request.Password
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult>(
            u => Ok(u),
            e => e.ToObjectResult());
    }

    [HttpGet("login/facebook")]
    public IActionResult LoginFacebook()
    {
        var redirectUrl = Url.Action("FacebookCallback", "Users", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        Console.WriteLine($"Initiating Facebook login, redirecting to: {redirectUrl}");
        return Challenge(properties, "Facebook");
    }

    [HttpGet("facebook-callback")]
    public async Task<IActionResult> FacebookCallback()
    {
        // Check for OAuth error parameters (e.g., when user cancels)
        var error = Request.Query["error"];
        var errorDescription = Request.Query["error_description"];
        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Facebook OAuth error: {error} - {errorDescription}");
            // Redirect back to frontend login with an error query param
            var redirectUrl = $"http://localhost:5173/login?error={Uri.EscapeDataString(error)}&error_description={Uri.EscapeDataString(errorDescription)}";
            return Redirect(redirectUrl);
        }
        
        var result = await HttpContext.AuthenticateAsync("Facebook");
        if (!result.Succeeded)
        {
            // If authentication fails or token is already present, check for token in redirect
            var tokenValue = Request.Query["token"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                // Token already issued, redirect to frontend directly
                var redirectUrl = $"http://localhost:5173/login?token={Uri.EscapeDataString(tokenValue)}";
                return Redirect(redirectUrl);
            }
            Console.WriteLine("Facebook authentication failed");
            return BadRequest("Facebook authentication failed");
        }

        var claims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
        Console.WriteLine("Facebook claims: " + string.Join(", ", claims));

        var email = result.Principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        var name = result.Principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
        name = name?.Replace(' ', '_');
        var facebookId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(facebookId))
        {
            Console.WriteLine("No email or Facebook ID provided");
            return BadRequest("Neither email nor Facebook ID provided by Facebook");
        }

        var userEmail = email ?? $"{facebookId}@facebook.com";
        var user = await userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            user = new User
            {
                UserName = name ?? facebookId,
                Email = userEmail
            };
            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                Console.WriteLine("User creation failed: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return BadRequest(createResult.Errors);
            }
            await userManager.AddToRoleAsync(user, "User");
        }
        var userRole = await userManager.GetRolesAsync(user);
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var tokenString = _jwtProvider.GenerateToken(user, userRole.ToList());

        // Redirect to frontend login page with token
        var redirect = $"http://localhost:5173/login?token={Uri.EscapeDataString(tokenString)}";
        Console.WriteLine($"Redirecting to: {redirect}");
        return Redirect(redirect);
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
    public async Task<ActionResult<UserDto>> Update([FromBody] UserUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateUserCommand
        {
            UserId = request.Id,
            UserName = request.UserName,
            Email = request.Email,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            user => UserDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }

    [HttpPut("{userId:guid}/update-roles")]
    public async Task<ActionResult> UpdateUserRoles([FromRoute] Guid userId, [FromBody] IList<string> roles)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound("User not found.");

        var existingRoles = await roleManager.Roles
            .Where(r => roles.Contains(r.Name))
            .Select(r => r.Name)
            .ToListAsync();

        var invalidRoles = roles.Except(existingRoles).ToList();

        if (invalidRoles.Any())
        {
           return BadRequest("Invalid roles: " + string.Join(", ", invalidRoles));
        }
        
        var currentRoles = await userManager.GetRolesAsync(user);
        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return BadRequest("Failed to remove existing roles.");
        

        var addResult = await userManager.AddToRolesAsync(user, existingRoles);
        if (!addResult.Succeeded)
            return BadRequest("Failed to assign new roles.");

        return Ok("Roles updated successfully.");
    }

    [HttpDelete("delete/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var input = new DeleteUserCommand
        {
            UserId = userId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}