using API.DTOs;
using API.DTOs.Common;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Roles.Commands;
using Domain.Models.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace API.Controllers;

[Route("roles")]
[ApiController]
public class RolesController(ISender sender, RoleManager<Role> roleManager) : ControllerBase
{
    [HttpGet("get-role-groups")]
    [Authorize(Roles = "Admin")]
    public ActionResult<List<object>> GetRoleGroups()
    {
        var roleGroups = Enum.GetValues(typeof(RoleGroups))
            .Cast<RoleGroups>()
            .Select(roleGroup => new
            {
                Id = (int)roleGroup,
                Name = roleGroup.ToString()
            })
            .ToList();

        return Ok(roleGroups);
    }

    [HttpGet("get-all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<RoleDto>>> GetAllRoles(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        return roles.Select(RoleDto.FromDomainModel).ToList();
    }

    [HttpGet("get-all-paginated")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginatedResponse<RoleDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = roleManager.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => r.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var roles = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var roleDtos = roles.Select(RoleDto.FromDomainModel).ToList();
        return Ok(new PaginatedResponse<RoleDto>
        {
            Items = roleDtos,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize
        });
    }

    [HttpGet("get-general-roles")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<RoleDto>>> GetGeneralRoles(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.Where(x => x.RoleGroup == RoleGroups.General)
            .ToListAsync(cancellationToken);
        return roles.Select(RoleDto.FromDomainModel).ToList();
    }

    [HttpGet("get-project-roles")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<List<RoleDto>>> GetProjectRoles(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.Where(x => x.RoleGroup == RoleGroups.Projects)
            .ToListAsync(cancellationToken);
        return roles.Select(RoleDto.FromDomainModel).ToList();
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleDto>> Create([FromBody] RoleCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateRoleCommand
        {
            Name = request.Name,
            RoleGroup = request.RoleGroup
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RoleDto>>(
            u => RoleDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleDto>> Update([FromBody] RoleDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateRoleCommand
        {
            Id = request.Id.Value,
            Name = request.Name,
            RoleGroup = request.RoleGroup
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RoleDto>>(
            user => RoleDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }

    [HttpDelete("delete/{roleId:guid}")]
    [Authorize(Roles = "Admin")]
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