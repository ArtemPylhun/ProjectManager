namespace Application.Users.Exceptions;

public class UserException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid UserId { get; } = id;
}

public class UserNotFoundException(Guid id) : UserException(id, $"User under id: {id} not found!");

public class UserWithNameAlreadyExistsException(Guid id, string username) : UserException(id, $"User under such userName: \"{username}\" already exists!");

public class UserWithEmailAlreadyExistsException(Guid id, string email)
    : UserException(id, $"User under such email: \"{email}\" already exists!");
    
public class InvalidCredentialsException() : UserException(Guid.Empty, "Invalid credentials!");
public class RoleNotFound(Guid id) : UserException(id, "This user doesn't have a role!");

public class UserUnknownException(Guid id, Exception innerException)
    : UserException(id, $"Unknown exception for the User under id: {id}!", innerException);
    
public class TokenExpiredException() : UserException(Guid.Empty, "Token expired!");
    
public class InvalidTokenException() : UserException(Guid.Empty, "Invalid token!");