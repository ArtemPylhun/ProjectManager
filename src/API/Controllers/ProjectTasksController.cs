using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.ProjectTasks.Commands;
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
        var projects = await projectTaskQueries.GetAll(cancellationToken);
        return projects.Select(ProjectTaskDto.FromDomainModel).ToList();
    }

    [HttpPost("create")]
    public async Task<ActionResult<ProjectTaskDto>> Create([FromBody] ProjectTaskCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateProjectTaskCommand
        {
            Name = request.Name,
            ProjectId = request.ProjectId.Value,
            EstimatedTime = request.EstimatedTime
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProjectTaskDto>>(
            p => ProjectTaskDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
    public async Task<ActionResult<ProjectTaskDto>> Create([FromBody] ProjectTaskUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateProjectTaskCommand
        {
            ProjectTaskId = request.Id.Value,
            ProjectId = request.ProjectId.Value,
            Name = request.Name,
            EstimatedTime = request.EstimatedTime
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
    public async Task<ActionResult<UserTaskDto>> AddUserToProject([FromBody] UserTaskCreateDto request,
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
    public async Task<ActionResult<UserTaskDto>> RemoveUserFromProject([FromRoute] Guid userTaskId,
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