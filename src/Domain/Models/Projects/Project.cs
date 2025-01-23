using Domain.Models.ProjectTasks;
using Domain.Models.ProjectUsers;
using Domain.Models.TimeEntries;
using Domain.Models.Users;

namespace Domain.Models.Projects;

public class Project
{
    public ProjectId Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatorId { get; private set; }
    public User? Creator { get; }
    
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    private Project(ProjectId id, string name, string description, DateTime createdAt, Guid creatorId)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        CreatorId = creatorId;
    }

    public static Project New(ProjectId id, string name, string description, DateTime createdAt,
        Guid userId) => new(id, name, description, createdAt, userId);
}