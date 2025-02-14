using System.Text.Json.Serialization;
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
    public string ColorHex { get; private set; }
    //TODO: make client optional
    public Guid ClientId { get; private set; }
    public User? Client { get; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    private Project(ProjectId id, string name, string description, DateTime createdAt, Guid creatorId, string colorHex,
        Guid clientId)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        CreatorId = creatorId;
        ColorHex = colorHex;
        ClientId = clientId;
    }

    public static Project New(ProjectId id, string name, string description, DateTime createdAt,
        Guid userId, string colorHex, Guid clientId) =>
        new(id, name, description, createdAt, userId, colorHex, clientId);

    public void UpdateDetails(string name, string description, string colorHex, Guid clientId)
    {
        Name = name;
        Description = description;
        ColorHex = colorHex;
        ClientId = clientId;
        
    }
}