using Application.Projects.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ProjectErrorHandler
{
    public static ObjectResult ToObjectResult(this ProjectException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                ProjectNotFoundException or CreatorNotFoundException
                  or ClientNotFoundException => StatusCodes
                    .Status404NotFound,
                ProjectAlreadyExistsException => StatusCodes
                    .Status409Conflict,
                ProjectUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Project error handler is not implemented")
            }
        };
    }
}