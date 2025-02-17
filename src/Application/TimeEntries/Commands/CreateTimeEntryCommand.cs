using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Projects.Exceptions;
using Application.TimeEntries.Exceptions;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;
using Domain.Models.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.TimeEntries.Commands;

public record CreateTimeEntryCommand : IRequest<Result<TimeEntry, TimeEntryException>>
{
    public string Description { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int Minutes { get; init; }
    public Guid UserId { get; init; }
    public ProjectId ProjectId { get; init; }
    public ProjectTaskId? ProjectTaskId { get; init; }
}

public class
    CreateTimeEntryCommandHandler : IRequestHandler<CreateTimeEntryCommand, Result<TimeEntry, TimeEntryException>>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly ITimeEntryQueries _timeEntryQueries;
    private readonly IProjectQueries _projectQueries;
    private readonly IProjectTaskQueries _projectTaskQueries;
    private readonly UserManager<User> _userManager;

    public CreateTimeEntryCommandHandler(ITimeEntryRepository timeEntryRepository, ITimeEntryQueries timeEntryQueries,
        IProjectQueries projectQueries, IProjectTaskQueries projectTaskQueries, UserManager<User> userManager)
    {
        _timeEntryRepository = timeEntryRepository;
        _timeEntryQueries = timeEntryQueries;
        _projectQueries = projectQueries;
        _projectTaskQueries = projectTaskQueries;
        _userManager = userManager;
    }

    public async Task<Result<TimeEntry, TimeEntryException>> Handle(CreateTimeEntryCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return await Task.FromResult(
                Result<TimeEntry, TimeEntryException>.Failure(new TimeEntryUserNotFoundException(request.UserId)));
        }

        var project = await _projectQueries.GetById(request.ProjectId, cancellationToken);
        return await project.Match(
            async p =>
            {
                if (request.StartTime > request.EndTime)
                {
                    return await Task.FromResult(
                        Result<TimeEntry, TimeEntryException>.Failure(
                            new TimeEntryEndDateMustBeAfterStartDate(request.StartTime, request.EndTime)));
                }

                if (request.ProjectTaskId != null)
                {
                    var projectTask = await _projectTaskQueries.GetById(request.ProjectTaskId, cancellationToken);
                    return await projectTask.Match(async pt =>
                        {
                            TimeEntry newTimeEntry = TimeEntry.New(TimeEntryId.New(), request.Description,
                                request.StartTime,
                                request.EndTime, request.Minutes, user.Id, p.Id, pt.Id);
                            return await CreateEntity(newTimeEntry, cancellationToken);
                        },
                        async () => await Task.FromResult(
                            Result<TimeEntry, TimeEntryException>.Failure(
                                new TimeEntryProjectNotFoundException(TimeEntryId.Empty(), p.Id))));
                }

                TimeEntry newTimeEntry = TimeEntry.New(TimeEntryId.New(), request.Description, request.StartTime,
                    request.EndTime, request.Minutes, user.Id, p.Id, ProjectTaskId.Empty());
                return await CreateEntity(newTimeEntry, cancellationToken);
            },
            async () => await Task.FromResult(
                Result<TimeEntry, TimeEntryException>.Failure(
                    new TimeEntryProjectNotFoundException(TimeEntryId.Empty(), ProjectId.Empty()))));
    }

    private async Task<Result<TimeEntry, TimeEntryException>> CreateEntity(
        TimeEntry entity,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _timeEntryRepository.Add(entity, cancellationToken);

            return result;
        }
        catch (Exception exception)
        {
            return new TimeEntryUnknownException(TimeEntryId.Empty(), exception);
        }
    }
}