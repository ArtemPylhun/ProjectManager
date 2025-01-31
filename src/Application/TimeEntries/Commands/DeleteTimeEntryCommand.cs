using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.TimeEntries.Exceptions;
using Domain.Models.TimeEntries;
using MediatR;

namespace Application.TimeEntries.Commands;

public record DeleteTimeEntryCommand: IRequest<Result<TimeEntry,TimeEntryException>>
{
    public Guid Id { get; init; }
    
}

public class
    DeleteTimeEntryCommandHandler : IRequestHandler<DeleteTimeEntryCommand, Result<TimeEntry, TimeEntryException>>
{
    private readonly ITimeEntryQueries _timeEntryQueries;
    private readonly ITimeEntryRepository _timeEntryRepository;

    public DeleteTimeEntryCommandHandler(ITimeEntryRepository timeEntryRepository, ITimeEntryQueries timeEntryQueries)
    {
        _timeEntryRepository = timeEntryRepository;
        _timeEntryQueries = timeEntryQueries;
    }

    public async Task<Result<TimeEntry, TimeEntryException>> Handle(DeleteTimeEntryCommand request,
        CancellationToken cancellationToken)
    {
        var existingTimeEntryId = new TimeEntryId(request.Id);
        var existingTimeEntry = await _timeEntryQueries.GetById(existingTimeEntryId, cancellationToken);
        return await existingTimeEntry.Match(
            async entry => await DeleteEntity(entry, cancellationToken),
            () => Task.FromResult(
                Result<TimeEntry, TimeEntryException>.Failure(new TimeEntryNotFoundException(existingTimeEntryId)))
        );
    }

    private async Task<Result<TimeEntry, TimeEntryException>> DeleteEntity(
        TimeEntry entity,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _timeEntryRepository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new TimeEntryUnknownException(TimeEntryId.Empty(), exception);
        }
    }
}