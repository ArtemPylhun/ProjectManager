namespace Application.Users.Exceptions;

public class UserException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid UserId { get; } = id;
}

public class UserNotFoundException() : UserException(Guid.Empty, $"User with not found!");

public class UserWithNameAlreadyExistsException(Guid id, string username) : UserException(id, $"User under such userName: \"{username}\" already exists!");

public class UserWithEmailAlreadyExistsException(Guid id, string email)
    : UserException(id, $"User under such email: \"{email}\" already exists!");
    
public class InvalidCredentialsException() : UserException(Guid.Empty, "Invalid credentials!");

public class UserUnknownException(Guid id, Exception innerException)
    : UserException(id, $"Unknown exception for the User under id: {id}!", innerException);

public class UserRolesNotFoundException(Guid id) : UserException(id, "This user doesn't have any roles!");
    
public class TokenExpiredException() : UserException(Guid.Empty, "Token expired!");
    
public class InvalidTokenException() : UserException(Guid.Empty, "Invalid token!");

public class UserAlreadyUsedInProject(Guid id) : UserException(id, "This User is already used in projects!");

public class EmailNotVerifiedException(Guid id)
    : UserException(id, $"User email is not verified!");

public class InvalidVerificationTokenException(Guid id)
    : UserException(id, $"Invalid verification token!");

public class EmailVerificationTokenExpiredException(Guid id)
    : UserException(id, $"Email verification token expired!");