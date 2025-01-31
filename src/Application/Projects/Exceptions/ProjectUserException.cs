using Domain.Models.Projects;
using Domain.Models.ProjectUsers;

namespace Application.Projects.Exceptions;

public class ProjectUserException(ProjectUserId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public ProjectUserId ProjectUserId { get; } = id;
}

public class ProjectUserNotFoundException(ProjectUserId id)
    : ProjectUserException(id, $"Project user under id: {id} not found!");

public class ProjectUserUnknownException(ProjectUserId id, Exception innerException)
    : ProjectUserException(id, $"Unknown exception for the Project user under id: {id}!", innerException);

public class ProjectUserAlreadyExistsException(ProjectUserId id)
    : ProjectUserException(id, $"Project user already exists: {id}!");

public class ProjectUserUserNotFoundException(ProjectUserId id, Guid userId)
    : ProjectUserException(id, $"Project user under id: {userId} not found!");
public class ProjectUserRoleNotFoundException(ProjectUserId id, Guid roleId)
    : ProjectUserException(id, $"Project role under id: {roleId} not found!");
public class ProjectUserProjectNotFoundException(ProjectUserId id, ProjectId projectId)
    : ProjectUserException(id, $"Project under id: {projectId} for this project user not found!");