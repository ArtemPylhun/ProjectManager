using Domain.Models.Projects;
using Domain.Models.TimeEntries;
using Domain.Models.UsersTasks;

namespace Domain.Models.ProjectTasks;

public class ProjectTask
{
    public ProjectTaskId Id { get; }

    public string Name { get; set; }
    public int EstimatedTime { get; set; }

    public string Description { get; set; }
    public ProjectTaskStatuses Status { get; set; }
    public ProjectId ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<UserTask> UsersTask { get; set; } = new List<UserTask>();

    public enum ProjectTaskStatuses
    {
        New,
        Development,
        Testing,
        ReturnedForRevision,
        Done
    }

    private ProjectTask(ProjectTaskId id, ProjectId projectId, string name, int estimatedTime, string description)
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        EstimatedTime = estimatedTime;
        Description = description;
        Status = ProjectTaskStatuses.New;
    }

    public static ProjectTask New(ProjectTaskId id, ProjectId projectId, string name, int estimatedTime, string description) =>
        new(id, projectId, name, estimatedTime, description);

    public void UpdateDetails(ProjectId projectId, string name, int estimatedTime, string description, ProjectTaskStatuses status)
    {
        ProjectId = projectId;
        Name = name;
        EstimatedTime = estimatedTime;
        Description = description;
        Status = status;
    }
}