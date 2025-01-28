namespace Application.Roles.Exceptions;

public class RoleException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Guid { get; } = id;
}

public class RoleNotFoundException(Guid id) : RoleException(id, $"Role under id: {id} not found!");

public class RoleAlreadyExistsException(Guid id) : RoleException(id, $"Role already exists: {id}!");

public class RoleUnknownException(Guid id, Exception innerException)
    : RoleException(id, $"Unknown exception for the Role under id: {id}!", innerException);