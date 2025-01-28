using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Roles.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;



[Route("roles")]
[ApiController]
public class RolesController(ISender sender, IRoleQueries roleQueries) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<List<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await roleQueries.GetAll(cancellationToken);
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