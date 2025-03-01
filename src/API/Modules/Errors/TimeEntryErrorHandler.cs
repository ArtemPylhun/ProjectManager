using Application.TimeEntries.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class TimeEntryErrorHandler
{
    public static ObjectResult ToObjectResult(this TimeEntryException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                TimeEntryNotFoundException or TimeEntryProjectNotFoundException or TimeEntryProjectTaskNotFoundException
                    or TimeEntryUserNotFoundException => StatusCodes
                        .Status404NotFound,
                TimeEntryAlreadyExistsException or TimeEntryEndDateMustBeAfterStartDate or TimeEntryOverlapException => StatusCodes
                    .Status409Conflict,
                TimeEntryUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("TimeEntry error handler is not implemented")
            }
        };
    }
}