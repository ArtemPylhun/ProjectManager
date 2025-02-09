using API.DTOs;
using Api.Modules.Errors;
using Application.Users.Commands;
using Domain.Models.Roles;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("users")]
[ApiController]
public class UsersController(ISender sender, UserManager<User> userManager, RoleManager<Role> roleManager) : ControllerBase
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
            Password = request.Password
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