using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Projects.Commands;
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
            ColorHex = request.ColorHex,
            ClientId = request.ClientId,
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
}