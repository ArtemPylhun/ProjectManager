using API.DTOs;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.TimeEntries.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("time-entries")]
[ApiController]
public class TimeEntriesController(ISender sender, ITimeEntryQueries roleQueries) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<List<TimeEntryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await roleQueries.GetAll(cancellationToken);
        return roles.Select(TimeEntryDto.FromDomainModel).ToList();
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<TimeEntryDto>> Create([FromBody] TimeEntryCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateTimeEntryCommand
        {
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Minutes = request.Minutes,
            UserId = request.UserId,
            ProjectId = request.ProjectId,
            ProjectTaskId = request.ProjectTaskId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TimeEntryDto>>(
            u => TimeEntryDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
    public async Task<ActionResult<TimeEntryDto>> Update([FromBody] TimeEntryUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateTimeEntryCommand
        {
            Id = request.Id.Value,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Minutes = request.Minutes,
            UserId = request.UserId,
            ProjectId = request.ProjectId,
            ProjectTaskId = request.ProjectTaskId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TimeEntryDto>>(
            user => TimeEntryDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{timeEntryId:guid}")]
    public async Task<ActionResult<TimeEntryDto>> Delete([FromRoute] Guid timeEntryId, CancellationToken cancellationToken)
    {
        var input = new DeleteTimeEntryCommand
        {
            Id = timeEntryId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TimeEntryDto>>(
            u => TimeEntryDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}