using Domain.Models.Projects;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Users;

public class User: IdentityUser<Guid>
{
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    
}