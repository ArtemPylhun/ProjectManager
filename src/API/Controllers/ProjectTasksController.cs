using API.DTOs;
using API.DTOs.Common;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.ProjectTasks.Commands;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("project-tasks")]
[ApiController]
public class ProjectTasksController(ISender sender, IProjectTaskQueries projectTaskQueries) : ControllerBase
{
    [HttpGet("get-all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ProjectTaskDto>>> GetAll(CancellationToken cancellationToken)
    {
        var projectTasks = await projectTaskQueries.GetAll(cancellationToken);
        return Ok(projectTasks.Select(ProjectTaskDto.FromDomainModel));
    }
    
    [HttpGet("get-all-by-user-id/{userId:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<List<ProjectTaskDto>>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var projectTasks = await projectTaskQueries.GetAllByUserId(userId, cancellationToken);
        return Ok(projectTasks.Select(ProjectTaskDto.FromDomainModel));
    }
    
    [HttpGet("get-all-paginated")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        var (projectTasks, totalCount) = await projectTaskQueries.GetAllPaginated(page, pageSize, search ?? "", cancellationToken);
        var projectTaskDtos = projectTasks.Select(ProjectTaskDto.FromDomainModel).ToList();
        return Ok(new PaginatedResponse<ProjectTaskDto>
        {
            Items = projectTaskDtos,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize
        });
    }

    [HttpGet("get-all-by-user-id-paginated/{userId:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAllByUserIdPaginated([FromRoute] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, CancellationToken cancellationToken = default)
    {
        var (projectTasks, totalCount) = await projectTaskQueries.GetAllByUserIdPaginated(userId, page, pageSize, search ?? "", cancellationToken);
        var projectTaskDtos = projectTasks.Select(ProjectTaskDto.FromDomainModel).ToList();
        return Ok(new PaginatedResponse<ProjectTaskDto>
        {
            Items = projectTaskDtos,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = pageSize
        });
    }
    
    [HttpGet("get-all-by-project-id/{projectId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ProjectTaskDto>>> GetAllByProjectId(Guid projectId, CancellationToken cancellationToken = default)
    {
        var projectIdValue = new ProjectId(projectId); 
        var projectTasks = await projectTaskQueries.GetAllByProjectId(projectIdValue, cancellationToken);
        return Ok(projectTasks);

    }
    
    [HttpGet("get-project-tasks-statuses")]
    [Authorize(Roles = "Admin,User")]
    public ActionResult<List<object>> GetProjectTaskStatuses()
    {
        var roleGroups = Enum.GetValues(typeof(ProjectTask.ProjectTaskStatuses))
            .Cast<ProjectTask.ProjectTaskStatuses>()
            .Select(projectTaskStatus => new 
            {
                Id = (int)projectTaskStatus,
                Name = projectTaskStatus.ToString()
            })
            .ToList();
    
        return Ok(roleGroups);
    }
    
    [HttpGet("{projectTaskId:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<ProjectTaskDto>> GetById([FromRoute] Guid projectTaskId, CancellationToken cancellationToken)
    {
        var projectTask = await projectTaskQueries.GetById(new ProjectTaskId(projectTaskId), cancellationToken);
        return projectTask.Match<ActionResult<ProjectTaskDto>>(
            p => ProjectTaskDto.FromDomainModel(p),
            () => NotFound());
    }
    
    [HttpPost("create")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<ProjectTaskDto>> Create([FromBody] ProjectTaskCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateProjectTaskCommand
        {
            Name = request.Name,
            ProjectId = request.ProjectId,
            CreatorId = request.CreatorId,
            EstimatedTime = request.EstimatedTime,
            Description = request.Description
            
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProjectTaskDto>>(
            p => ProjectTaskDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }
    
    [HttpPut("update")]
    [Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<ProjectTaskDto>> Update([FromBody] ProjectTaskUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateProjectTaskCommand
        {
            ProjectTaskId = request.Id,
            Name = request.Name,
            EstimatedTime = request.EstimatedTime,
            Description = request.Description,
            ProjectId = request.ProjectId,
            Status = request.Status
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProjectTaskDto>>(
            p => ProjectTaskDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProjectTaskDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteProjectTaskCommand
        {
            ProjectTaskId = id
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<ProjectTaskDto>>(
            ev => ProjectTaskDto.FromDomainModel(ev),
            e => e.ToObjectResult());
    }
    }