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
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectDto request, CancellationToken cancellationToken)
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
            u => Ok(u),
            e => e.ToObjectResult());
    }
}