using Domain.Models.Projects;
using Domain.Models.ProjectTasks;

namespace Tests.Data;

public static class ProjectTasksData
{
    public static ProjectTask NewProjectTask(ProjectId projectId) =>
    ProjectTask.New(ProjectTaskId.New(), projectId, "NewProjectTask", 1, "Description for task");
    
    public static ProjectTask ExistingProjectTask(ProjectId projectId) =>
        ProjectTask.New(ProjectTaskId.New(), projectId, "ExistingProjectTask", 1, "Description for task");
    public static ProjectTask ExistingProjectTask2(ProjectId projectId) =>
        ProjectTask.New(ProjectTaskId.New(), projectId, "ExistingProjectTask2", 1, "Description for task");
}