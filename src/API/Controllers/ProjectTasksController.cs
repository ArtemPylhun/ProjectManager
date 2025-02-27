using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.ProjectTasks.Commands;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("project-tasks")]
[ApiController]
public class ProjectTasksController(ISender sender, IProjectTaskQueries projectTaskQueries) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<List<ProjectTaskDto>>> GetAll(CancellationToken cancellationToken)
    {
        var projectTasks = await projectTaskQueries.GetAll(cancellationToken);
        return Ok(projectTasks.Select(ProjectTaskDto.FromDomainModel));
    }
    
    [HttpGet("get-all-by-user-id/{userId:guid}")]
    public async Task<ActionResult<List<ProjectTaskDto>>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var projectTasks = await projectTaskQueries.GetAllByUserId(userId, cancellationToken);
        return Ok(projectTasks.Select(ProjectTaskDto.FromDomainModel));
    }
    
    [HttpGet("get-all-paginated")]
    public async Task<IActionResult> GetAllPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (defaultProjectTasks, totalCount) = await projectTaskQueries.GetAllPaginated(page, pageSize, cancellationToken);
        var projectTasks = defaultProjectTasks.Select(ProjectTaskDto.FromDomainModel).ToList();
        return Ok(new { projectTasks, totalCount });
    }
    
    [HttpGet("get-all-by-user-id-paginated/{userId:guid}")]
    public async Task<ActionResult<List<ProjectTaskDto>>> GetAllByUserIdPaginated(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (defaultProjectTasks, totalCount) = await projectTaskQueries.GetAllByUserIdPaginated(userId, page, pageSize, cancellationToken);
        var projectTasks = defaultProjectTasks.Select(ProjectTaskDto.FromDomainModel).ToList();
        return Ok(new { projectTasks, totalCount });

    }
    
    [HttpGet("get-all-by-project-id/{projectId:guid}")]
    public async Task<ActionResult<List<ProjectTaskDto>>> GetAllByProjectId(Guid projectId, CancellationToken cancellationToken = default)
    {
        var projectIdValue = new ProjectId(projectId); 
        var projectTasks = await projectTaskQueries.GetAllByProjectId(projectIdValue, cancellationToken);
        return Ok(projectTasks);

    }
    
    [HttpGet("get-project-tasks-statuses")]
    public ActionResult<List<object>> GetRoleGroups()
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
    public async Task<ActionResult<ProjectTaskDto>> GetById([FromRoute] Guid projectTaskId, CancellationToken cancellationToken)
    {
        var projectTask = await projectTaskQueries.GetById(new ProjectTaskId(projectTaskId), cancellationToken);
        return projectTask.Match<ActionResult<ProjectTaskDto>>(
            p => ProjectTaskDto.FromDomainModel(p),
            () => NotFound());
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<ProjectTaskDto>> Create([FromBody] ProjectTaskCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateProjectTaskCommand
        {
            Name = request.Name,
            ProjectId = request.ProjectId,
            EstimatedTime = request.EstimatedTime,
            Description = request.Description
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProjectTaskDto>>(
            p => ProjectTaskDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
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

    [HttpPost("add-user-to-project-task")]
    public async Task<ActionResult<UserTaskDto>> AddUserToProjectTask([FromBody] UserTaskCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new AddUserToProjectTaskCommand
        {
            ProjectTaskId = request.ProjectTaskId,
            UserId = request.UserId,
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<UserTaskDto>>(
            pu => UserTaskDto.FromDomainModel(pu),
            e => e.ToObjectResult());
    }

    [HttpDelete("remove-user-from-project-task/{userTaskId:guid}")]
    public async Task<ActionResult<UserTaskDto>> RemoveUserFromProjectTask([FromRoute] Guid userTaskId,
        CancellationToken cancellationToken)
    {
        var input = new RemoveUserFromProjectTaskCommand
        {
            UserTaskId = userTaskId
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<UserTaskDto>>(
            pu => UserTaskDto.FromDomainModel(pu),
            e => e.ToObjectResult());
    }
}