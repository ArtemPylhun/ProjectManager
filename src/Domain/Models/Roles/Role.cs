using Domain.Models.ProjectUsers;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Roles;

public class Role: IdentityRole<Guid>
{
    public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
}