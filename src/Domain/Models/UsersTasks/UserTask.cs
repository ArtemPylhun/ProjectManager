using System.Text.Json.Serialization;
using Domain.Models.ProjectTasks;
using Domain.Models.Users;

namespace Domain.Models.UsersTasks;

public class UserTask
{
    public UserTaskId Id { get; }
    public ProjectTaskId ProjectTaskId { get; private set; }
    [JsonIgnore]
    public ProjectTask? ProjectTask { get; }
    public Guid UserId { get; private set; }
    [JsonIgnore]
    public User? User { get; }

    private UserTask(UserTaskId id, ProjectTaskId projectTaskId, Guid userId)
    {
        Id = id;
        ProjectTaskId = projectTaskId;
        UserId = userId;
    }

    public static UserTask New(UserTaskId id, ProjectTaskId projectTaskId, Guid userId) => 
        new(id, projectTaskId, userId);

    public void UpdateDetails(ProjectTaskId projectTaskId, Guid userId)
    {
        ProjectTaskId = projectTaskId;
        UserId = userId;
    }
}