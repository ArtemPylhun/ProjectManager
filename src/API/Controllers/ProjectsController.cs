using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Projects.Commands;
using Domain.Models.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("projects")]
[ApiController]
public class ProjectsController(ISender sender, IProjectQueries projectQueries): ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<List<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        var projects = await projectQueries.GetAll(cancellationToken);
        return projects.Select(ProjectDto.FromDomainModel).ToList();
    }
    
    [HttpGet("{projectId:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById([FromRoute] Guid projectId, CancellationToken cancellationToken)
    {
        var project = await projectQueries.GetById(new ProjectId(projectId), cancellationToken);
        return project.Match<ActionResult<ProjectDto>>(
            p => ProjectDto.FromDomainModel(p),
            () => NotFound());
    }
    [HttpPost("create")]
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
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectUpdateDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateProjectCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            ColorHex = request.ColorHex
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<ProjectDto>>(
            p => ProjectDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{id:guid}")]
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