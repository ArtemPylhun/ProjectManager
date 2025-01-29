using Application.ProjectTasks.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ProjectTaskErrorHandler
{
    public static ObjectResult ToObjectResult(this ProjectTaskException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                ProjectTaskNotFoundException or ProjectForTaskNotFoundException => StatusCodes
                        .Status404NotFound,
                ProjectTaskAlreadyExistsException => StatusCodes
                    .Status409Conflict,
                ProjectTaskUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("ProjectTask error handler is not implemented")
            }
        };
    }
}