using Domain.Models.ProjectTasks;
using Domain.Models.UsersTasks;

namespace Tests.Data;

public static class UserTasksData
{
    public static UserTask NewUserTask(ProjectTaskId projectTaskId, Guid userId) => 
        UserTask.New(UserTaskId.New(), projectTaskId, userId);
    
    public static UserTask ExistingUserTask(ProjectTaskId projectTaskId, Guid userId) => 
        UserTask.New(UserTaskId.New(), projectTaskId, userId);
}