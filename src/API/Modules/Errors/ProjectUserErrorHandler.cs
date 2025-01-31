using Application.Projects.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ProjectUserErrorHandler
{
    public static ObjectResult ToObjectResult(this ProjectUserException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                ProjectUserNotFoundException or ProjectUserProjectNotFoundException or ProjectUserUserNotFoundException
                    or ProjectUserRoleNotFoundException => StatusCodes
                        .Status404NotFound,
                ProjectUserAlreadyExistsException => StatusCodes
                    .Status409Conflict,
                ProjectUserUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Project user error handler is not implemented")
            }
        };
    }
}