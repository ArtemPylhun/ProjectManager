using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.UsersTasks;

namespace Application.ProjectTasks.Exceptions;

public class UserTaskException(UserTaskId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public UserTaskId UserTaskId { get; } = id;
}

public class UserTaskNotFoundException(UserTaskId id)
    : UserTaskException(id, $"Task user under id: {id} not found!");

public class UserTaskUnknownException(UserTaskId id, Exception innerException)
    : UserTaskException(id, $"Unknown exception for the task user under id: {id}!", innerException);

public class UserTaskAlreadyExistsException(UserTaskId id)
    : UserTaskException(id, $"Task user already exists: {id}!");

public class UserTaskUserNotFoundException(UserTaskId id, Guid userId)
    : UserTaskException(id, $"Task user under id: {userId} not found!");
public class UserTaskProjectTaskNotFoundException(UserTaskId id, ProjectTaskId taskId)
    : UserTaskException(id, $"Task under id: {taskId} for this project task user not found!");