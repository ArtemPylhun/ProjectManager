using Domain.Models.Projects;

namespace Application.Projects.Exceptions;

public class ProjectException(ProjectId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public ProjectId ProjectId { get; } = id;
}

public class ProjectNotFoundException(ProjectId id) : ProjectException(id, $"Project under id: {id} not found!");

public class ProjectAlreadyExistsException(ProjectId id, string name) : ProjectException(id, $"Project already exists: {name}!");

public class ProjectUnknownException(ProjectId id, Exception innerException )
    : ProjectException(id, $"Unknown exception for the project under id: {id}!", innerException );

public class CreatorNotFoundException(Guid creatorId)
:ProjectException(ProjectId.Empty(), $"Creator under id: {creatorId} in project not found!");

public class ClientNotFoundException(Guid clientId)
    :ProjectException(ProjectId.Empty(), $"Client under id: {clientId} not found!");