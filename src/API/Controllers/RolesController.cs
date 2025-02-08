using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Roles.Commands;
using Domain.Models.Roles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;



[Route("roles")]
[ApiController]
public class RolesController(ISender sender, RoleManager<Role> roleManager) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<List<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        return roles.Select(RoleDto.FromDomainModel).ToList();
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<RoleDto>> Create([FromBody] RoleDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateRoleCommand
        {
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RoleDto>>(
            u => RoleDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
    public async Task<ActionResult<RoleDto>> Update([FromBody] RoleDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateRoleCommand
        {
            Id = request.Id.Value,
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RoleDto>>(
            user => RoleDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{roleId:guid}")]
    public async Task<ActionResult<RoleDto>> Delete([FromRoute] Guid roleId, CancellationToken cancellationToken)
    {
        var input = new DeleteRoleCommand
        {
           Id = roleId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RoleDto>>(
            u => RoleDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}