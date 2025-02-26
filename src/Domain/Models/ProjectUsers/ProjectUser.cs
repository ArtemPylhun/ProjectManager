using System.Text.Json.Serialization;
using Domain.Models.Projects;
using Domain.Models.Roles;
using Domain.Models.Users;

namespace Domain.Models.ProjectUsers;

public class ProjectUser
{
    public ProjectUserId Id { get; }
    public ProjectId ProjectId { get; private set; }
    public Project? Project { get; }
    public Guid UserId { get; private set; }
    
    public User? User { get; }
    public Guid RoleId { get; private set; }
    public Role? Role { get; }

    private ProjectUser(ProjectUserId id, ProjectId projectId, Guid userId, Guid roleId)
    {
        Id = id;
        ProjectId = projectId;
        UserId = userId;
        RoleId = roleId;
    }

    public static ProjectUser New(ProjectUserId id, ProjectId projectId, Guid userId, Guid roleId) =>
        new(id, projectId, userId, roleId);
}