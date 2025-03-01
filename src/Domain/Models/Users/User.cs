using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.ProjectUsers;
using Domain.Models.TimeEntries;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Users;

public class User: IdentityUser<Guid>
{
    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public ICollection<Project> ClientProjects { get; set; } = new List<Project>();
    public ICollection<ProjectTask> CreatedProjectTasks { get; set; } = new List<ProjectTask>();
    
}