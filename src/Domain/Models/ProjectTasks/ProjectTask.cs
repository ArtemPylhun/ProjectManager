using Domain.Models.Projects;
using Domain.Models.TimeEntries;
using Domain.Models.UsersTasks;

namespace Domain.Models.ProjectTasks;

public class ProjectTask
{
    public ProjectTaskId Id { get; }
    
    public string Name { get; set; }
    public int EstimatedTime { get; set; }
    
    public ProjectId ProjectId { get; set; }
    public Project? Project { get; set; }
    
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<UserTask> UsersTask { get; set; } = new List<UserTask>();

    private ProjectTask(ProjectTaskId id,ProjectId projectId, string name, int estimatedTime)
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        EstimatedTime = estimatedTime;
    }
    
    public static ProjectTask New(ProjectTaskId id,ProjectId projectId, string name, int estimatedTime) => new(id, projectId, name, estimatedTime);
}