using System.Reflection;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.ProjectUsers;
using Domain.Models.Roles;
using Domain.Models.TimeEntries;
using Domain.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public DbSet<ProjectTask> ProjectTasks  { get; set; }
    public DbSet<ProjectUser> ProjectUsers  { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}