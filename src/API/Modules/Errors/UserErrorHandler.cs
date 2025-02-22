using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserErrorHandler
{
    public static ObjectResult ToObjectResult(this UserException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                UserNotFoundException or UserRolesNotFoundException => StatusCodes
                    .Status404NotFound,
                InvalidCredentialsException or TokenExpiredException => StatusCodes.Status401Unauthorized,
                UserWithNameAlreadyExistsException or UserWithEmailAlreadyExistsException or UserAlreadyUsedInProject => StatusCodes
                    .Status409Conflict,
                UserUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User error handler is not implemented")
            }
        };
    }
}