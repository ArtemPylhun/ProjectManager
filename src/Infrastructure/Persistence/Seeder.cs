using Domain.Models.ProjectTasks;
using Domain.Models.ProjectUsers;
using Domain.Models.Projects;
using Domain.Models.TimeEntries;
using Domain.Models.Users;
using Domain.Models.UsersTasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Domain.Models.Roles;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class Seeder
    {
        public static async Task SeedRoles(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            if (!context.Roles.Any())
            {
                var roles = new[]
                {
                    "Admin",
                    "User"
                };

                foreach (var role in roles)
                    await roleManager.CreateAsync(new Role { Name = role, RoleGroup = RoleGroups.General });

                roles =
                [
                    "Project manager",
                    "Project sponsor",
                    "Business analyst",
                    "Project team member",
                ];

                foreach (var role in roles)
                    await roleManager.CreateAsync(new Role { Name = role, RoleGroup = RoleGroups.Projects });

                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedUsers(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "adminUser",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                var usualUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "usualUser",
                    Email = "user@example.com",
                    EmailConfirmed = true
                };

                var password = "AdminPass123!";
                await userManager.CreateAsync(adminUser, password);
                await userManager.AddToRoleAsync(adminUser, "Admin");

                await userManager.CreateAsync(usualUser, password);
                await userManager.AddToRoleAsync(usualUser, "User");

                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedProjects(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (!context.Projects.Any())
            {
                var users = await context.Users.ToListAsync();
                var adminUser = users.FirstOrDefault(u => u.UserName == "adminUser");
                var usualUser = users.FirstOrDefault(u => u.UserName == "usualUser");

                if (adminUser == null || usualUser == null)
                {
                    throw new Exception("Users must be seeded before adding projects.");
                }

                var projects = new[]
                {
                    Project.New(ProjectId.New(), "Website Development", "Building a new company website",
                        DateTime.UtcNow, adminUser.Id, "#73aaee", usualUser.Id),
                    Project.New(ProjectId.New(), "Mobile App Upgrade", "Upgrading the mobile application",
                        DateTime.UtcNow.AddDays(1), adminUser.Id, "#b8d7fb", usualUser.Id)
                };

                await context.Projects.AddRangeAsync(projects);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedProjectTasks(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.ProjectTasks.Any())
            {
                var projects = await context.Projects.ToListAsync();

                if (!projects.Any())
                {
                    throw new Exception("Projects must be seeded before adding project tasks.");
                }

                var projectTasks = new[]
                {
                    ProjectTask.New(ProjectTaskId.New(), projects[0].Id, "Frontend Development", 240,
                        "Develop the website frontend"),
                    ProjectTask.New(ProjectTaskId.New(), projects[0].Id, "Backend Integration", 180,
                        "Integrate backend APIs"),
                    ProjectTask.New(ProjectTaskId.New(), projects[1].Id, "UI Redesign", 300, "Redesign mobile app UI")
                };

                await context.ProjectTasks.AddRangeAsync(projectTasks);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedProjectUsers(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            if (!context.ProjectUsers.Any())
            {
                var projects = await context.Projects.ToListAsync();
                var users = await context.Users.ToListAsync();
                var roles = await roleManager.Roles.ToListAsync();
                var projectRoles = roles.Where(x => x.RoleGroup == RoleGroups.Projects).ToList();
                var projectManagerRole = roles.FirstOrDefault(r => r.Name == "Project manager");
                var projectSponsorRole = roles.FirstOrDefault(r => r.Name == "Project sponsor");
                var teamMemberRole = projectRoles.FirstOrDefault(r => r.Name == "Project team member");
                if (!projects.Any() || !users.Any() || projectManagerRole == null || projectSponsorRole == null ||
                    teamMemberRole == null)
                {
                    throw new Exception("Projects, users, and roles must be seeded before adding project users.");
                }

                var adminUser = users.First(u => u.UserName == "adminUser");
                var usualUser = users.First(u => u.UserName == "usualUser");

                var projectUsers = new[]
                {
                    ProjectUser.New(ProjectUserId.New(), projects[0].Id, adminUser.Id, projectManagerRole.Id),
                    ProjectUser.New(ProjectUserId.New(), projects[0].Id, usualUser.Id, projectSponsorRole.Id),
                    ProjectUser.New(ProjectUserId.New(), projects[1].Id, usualUser.Id, teamMemberRole.Id)
                };

                await context.ProjectUsers.AddRangeAsync(projectUsers);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedUserTasks(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            if (!context.UserTasks.Any())
            {
                var projectTasks = await context.ProjectTasks.ToListAsync();
                var users = await context.Users.ToListAsync();

                if (!projectTasks.Any() || !users.Any())
                {
                    throw new Exception("Project tasks, users, and roles must be seeded before adding user tasks.");
                }

                var adminUser = users.First(u => u.UserName == "adminUser");
                var usualUser = users.First(u => u.UserName == "usualUser");

                var userTasks = new[]
                {
                    UserTask.New(UserTaskId.New(), projectTasks[0].Id, adminUser.Id),
                    UserTask.New(UserTaskId.New(), projectTasks[0].Id, usualUser.Id),
                    UserTask.New(UserTaskId.New(), projectTasks[1].Id, usualUser.Id)
                };

                await context.UserTasks.AddRangeAsync(userTasks);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedTimeEntries(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.TimeEntries.Any())
            {
                var projects = await context.Projects.ToListAsync();
                var projectTasks = await context.ProjectTasks.ToListAsync();
                var users = await context.Users.ToListAsync();

                if (!projects.Any() || !projectTasks.Any() || !users.Any())
                {
                    throw new Exception(
                        "Projects, project tasks, and users must be seeded before adding time entries.");
                }

                var adminUser = users.First(u => u.UserName == "adminUser");
                var usualUser = users.First(u => u.UserName == "usualUser");

                var timeEntries = new[]
                {
                    TimeEntry.New(TimeEntryId.New(), "Coding frontend", DateTime.UtcNow.AddHours(-2),
                        DateTime.UtcNow.AddHours(-1), 60, adminUser.Id, projects[0].Id, projectTasks[0].Id),
                    TimeEntry.New(TimeEntryId.New(), "Testing backend", DateTime.UtcNow.AddHours(-4),
                        DateTime.UtcNow.AddHours(-3), 60, usualUser.Id, projects[0].Id, projectTasks[1].Id),
                    TimeEntry.New(TimeEntryId.New(), "UI redesign", DateTime.UtcNow.AddHours(-6),
                        DateTime.UtcNow.AddHours(-5), 60, usualUser.Id, projects[1].Id, projectTasks[2].Id)
                };

                await context.TimeEntries.AddRangeAsync(timeEntries);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedAll(this IApplicationBuilder app)
        {
            await app.SeedRoles();
            await app.SeedUsers();
            await app.SeedProjects();
            await app.SeedProjectTasks();
            await app.SeedProjectUsers();
            await app.SeedUserTasks();
            await app.SeedTimeEntries();
        }
    }
}