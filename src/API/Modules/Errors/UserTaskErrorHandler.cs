using Application.ProjectTasks.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserTaskErrorHandler
{
    public static ObjectResult ToObjectResult(this UserTaskException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                UserTaskNotFoundException or UserTaskProjectTaskNotFoundException or UserTaskUserNotFoundException
                     => StatusCodes
                        .Status404NotFound,
                UserTaskAlreadyExistsException => StatusCodes
                    .Status409Conflict,
                UserTaskUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User task error handler is not implemented")
            }
        };
    }
}