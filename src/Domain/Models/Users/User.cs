using System.Text.Json.Serialization;
using Domain.Models.Projects;
using Domain.Models.ProjectUsers;
using Domain.Models.TimeEntries;
using Domain.Models.UsersTasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Users;

public class User: IdentityUser<Guid>
{
    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    [JsonIgnore]
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    [JsonIgnore]
    public ICollection<Project> ClientProjects { get; set; } = new List<Project>();
    
}