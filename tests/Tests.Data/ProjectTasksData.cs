using Domain.Models.Projects;
using Domain.Models.ProjectTasks;

namespace Tests.Data;

public static class ProjectTasksData
{
    public static ProjectTask NewProjectTask(ProjectId projectId, Guid userId) =>
    ProjectTask.New(ProjectTaskId.New(), projectId, "NewProjectTask", 1, "Description for task", DateTime.UtcNow, userId);
    
    public static ProjectTask ExistingProjectTask(ProjectId projectId, Guid userId) =>
        ProjectTask.New(ProjectTaskId.New(), projectId, "ExistingProjectTask", 1, "Description for task", DateTime.UtcNow, userId);
    public static ProjectTask ExistingProjectTask2(ProjectId projectId, Guid userId) =>
        ProjectTask.New(ProjectTaskId.New(), projectId, "ExistingProjectTask2", 1, "Description for task", DateTime.UtcNow, userId);
}