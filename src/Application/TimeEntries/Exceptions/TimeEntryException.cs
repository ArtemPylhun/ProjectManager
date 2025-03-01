using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;

namespace Application.TimeEntries.Exceptions;

public class TimeEntryException(TimeEntryId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public TimeEntryId TimeEntryId { get; } = id;
}

public class TimeEntryNotFoundException(TimeEntryId id)
    : TimeEntryException(id, $"Time entry under id: {id} not found!");

public class TimeEntryAlreadyExistsException(TimeEntryId id)
    : TimeEntryException(id, $"Time entry already exists: {id}!");

public class TimeEntryUnknownException(TimeEntryId id, Exception innerException)
    : TimeEntryException(id, $"Unknown exception for the project under id: {id} occured!", innerException);

public class TimeEntryProjectNotFoundException(TimeEntryId id, ProjectId projectId)
    : TimeEntryException(id, $"Time entry project under id: {projectId} not found!");

public class TimeEntryProjectTaskNotFoundException(TimeEntryId id, ProjectTaskId projectTaskId)
    : TimeEntryException(id, $"Time entry project task under id: {projectTaskId} not found!");

public class TimeEntryEndDateMustBeAfterStartDate(DateTime startDate, DateTime endDate)
    : TimeEntryException(TimeEntryId.Empty(),
        $"End date must be after start date. Start date: {startDate}. End date: {endDate}!");

public class TimeEntryUserNotFoundException(Guid userId)
    : TimeEntryException(TimeEntryId.Empty(), $"Time entry user under id: {userId} not found!");
    
public class TimeEntryOverlapException(DateTime startTime, DateTime endTime)
    : TimeEntryException(TimeEntryId.Empty(), $"Time entry overlap. Start date: {startTime}. End date: {endTime}!");