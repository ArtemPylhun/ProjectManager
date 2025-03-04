using API.DTOs;
using API.DTOs.Common;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Projects.Commands;
using Domain.Models.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("projects")]
[ApiController]
public class ProjectsController(ISender sender, IProjectQueries projectQueries): ControllerBase
{
    [HttpGet("get-all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<TimeEntryDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        var projects = await projectQueries.GetAll(cancellationToken);
        return Ok(projects.Select(ProjectDto.FromDomainModel));
    }
    
    [HttpGet("get-all-by-user-id/{userId:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<List<TimeEntryDto>>> GetAllByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        var projects = await projectQueries.GetAllByUserId(userId, cancellationToken);
        return Ok(projects.Select(ProjectDto.FromDomainModel));
    }
    
    [HttpGet("get-all-paginated")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        var (projects, totalCount) = await projectQueries.GetAllPaginated(page, pageSize, search ?? "", cancellationToken);
        var projectDtos = projects.Select(ProjectDto.FromDomainModel).ToList();
        return Ok(new PaginatedResponse<ProjectDto>
        {
            Items = projectDtos,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize
        });
    }

    [HttpGet("get-all-by-user-id-paginated/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAllByUserIdPaginated([FromRoute] string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        var (projects, totalCount) = await projectQueries.GetAllByUserIdPaginated(Guid.Parse(userId), page, pageSize, search ?? "", cancellationToken);
        var projectDtos = projects.Select(ProjectDto.FromDomainModel).ToList();
        return Ok(new PaginatedResponse<ProjectDto>
        {
            Items = projectDtos,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize
        });
    }
    
    [HttpGet("{projectId:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<ProjectDto>> GetById([FromRoute] Guid projectId, CancellationToken cancellationToken)
    {
        var project = await projectQueries.GetById(new ProjectId(projectId), cancellationToken);
        return project.Match<ActionResult<ProjectDto>>(
            p => ProjectDto.FromDomainModel(p),
            () => NotFound());
    }
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectCreateDto request, CancellationToken cancellationToken)
    {
        var input = new CreateProjectCommand
        {
            Name = request.Name,
            Description = request.Description,
            ColorHex = request.ColorHex,
            ClientId = request.ClientId,
            CreatorId = request.CreatorId
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<ProjectDto>>(
            p => ProjectDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }
    
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectUpdateDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateProjectCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            ColorHex = request.ColorHex,
            ClientId = request.ClientId
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<ProjectDto>>(
            p => ProjectDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteProjectCommand
        {
            Id = id
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<ProjectDto>>(
            ev => ProjectDto.FromDomainModel(ev),
            e => e.ToObjectResult());
    }
    
    [HttpPost("add-user-to-project")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectUserDto>> AddUserToProject([FromBody] ProjectUserCreateDto request, CancellationToken cancellationToken)
    {
        var input = new AddUserToProjectCommand
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            RoleId = request.RoleId
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<ProjectUserDto>>(
            pu => ProjectUserDto.FromDomainModel(pu),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("remove-user-from-project/{projectUserId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectUserDto>> RemoveUserFromProject([FromRoute] Guid projectUserId, CancellationToken cancellationToken)
    {
        var input = new RemoveUserFromProjectCommand
        {
            ProjectUserId = projectUserId
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<ProjectUserDto>>(
            pu => ProjectUserDto.FromDomainModel(pu),
            e => e.ToObjectResult());
    }
}