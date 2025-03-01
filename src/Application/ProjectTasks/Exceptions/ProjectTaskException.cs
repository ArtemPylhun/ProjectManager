using Domain.Models.Projects;
using Domain.Models.ProjectTasks;

namespace Application.ProjectTasks.Exceptions;

public class ProjectTaskException(ProjectTaskId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public ProjectTaskId ProjectTaskId { get; } = id;
}

public class ProjectTaskNotFoundException(ProjectTaskId id)
    : ProjectTaskException(id, $"Project task under id: {id} not found!");

public class ProjectTaskAlreadyExistsException(ProjectTaskId id, string projectName, string projectTaskName)
    : ProjectTaskException(id, $"Project task already exists: {projectTaskName} in the project: {projectName}!");

public class ProjectTaskUnknownException(ProjectTaskId id, Exception innerException)
    : ProjectTaskException(id, $"Unknown exception for the project under id: {id}!", innerException);

public class ProjectForTaskNotFoundException(ProjectTaskId id, ProjectId projectId)
    : ProjectTaskException(id, $"Project under id: {projectId} for this task not found!");
    
public class CreatorNotFoundException(Guid creatorId)
    :ProjectTaskException(ProjectTaskId.Empty(), $"Creator under id: {creatorId} in project task not found!");
