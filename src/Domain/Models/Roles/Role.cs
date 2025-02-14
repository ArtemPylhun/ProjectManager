using Domain.Models.ProjectUsers;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Roles;

public class Role : IdentityRole<Guid>
{
    public RoleGroups RoleGroup { get; set; }
    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    
}

public enum RoleGroups
{
    General,
    Projects
}