using Domain.Models.ProjectTasks;
using Domain.Models.ProjectUsers;
using Domain.Models.Projects;
using Domain.Models.TimeEntries;
using Domain.Models.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

                // Add 5 more users with varied roles
                var additionalUsers = new List<User>();
                var userNames = new[] { "manager1", "sponsor1", "analyst1", "team1", "user3" };
                var emails = userNames.Select(u => $"{u}@example.com").ToArray();
                var roles = new[] { "Project manager", "Project sponsor", "Business analyst", "Project team member", "User" };

                for (int i = 0; i < 5; i++)
                {
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = userNames[i],
                        Email = emails[i],
                        EmailConfirmed = true
                    };
                    additionalUsers.Add(user);
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, roles[i]);
                }

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

                var projects = new List<Project>();
                for (int i = 1; i <= 20; i++)
                {
                    projects.Add(Project.New(
                        ProjectId.New(),
                        $"Project {i}",
                        $"Description for Project {i}",
                        DateTime.UtcNow.AddDays(i),
                        adminUser.Id,
                        $"#{RandomColor()}",
                        usualUser.Id
                    ));
                }

                await context.Projects.AddRangeAsync(projects);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedProjectTasks(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (!context.ProjectTasks.Any())
            {
                var users = await context.Users.ToListAsync();
                var adminUser = users.FirstOrDefault(u => u.UserName == "adminUser");
                var usualUser = users.FirstOrDefault(u => u.UserName == "usualUser");
                var projects = await context.Projects.ToListAsync();

                if (!projects.Any())
                {
                    throw new Exception("Projects must be seeded before adding project tasks.");
                }

                var projectTasks = new List<ProjectTask>();
                var random = new Random();
                foreach (var project in projects)
                {
                    for (int i = 1; i <= 5; i++) // 5 tasks per project, total 100
                    {
                        var creator = random.Next(2) == 0 ? adminUser : usualUser;
                        projectTasks.Add(ProjectTask.New(
                            ProjectTaskId.New(),
                            project.Id,
                            $"Task {i} for {project.Name}",
                            random.Next(120, 480), // Estimated time between 2 and 8 hours
                            $"Description for Task {i} in {project.Name}",
                            DateTime.UtcNow.AddDays(random.Next(-30, 0)),
                            creator.Id
                        ));
                    }
                }

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
                var businessAnalystRole = roles.FirstOrDefault(r => r.Name == "Business analyst");
                var teamMemberRole = projectRoles.FirstOrDefault(r => r.Name == "Project team member");
                if (!projects.Any() || !users.Any() || projectManagerRole == null || projectSponsorRole == null ||
                    businessAnalystRole == null || teamMemberRole == null)
                {
                    throw new Exception("Projects, users, and roles must be seeded before adding project users.");
                }

                var adminUser = users.First(u => u.UserName == "adminUser");
                var usualUser = users.First(u => u.UserName == "usualUser");
                var manager1 = users.First(u => u.UserName == "manager1");
                var sponsor1 = users.First(u => u.UserName == "sponsor1");
                var analyst1 = users.First(u => u.UserName == "analyst1");
                var team1 = users.First(u => u.UserName == "team1");
                var user3 = users.First(u => u.UserName == "user3");

                var projectUsers = new List<ProjectUser>();
                var random = new Random();
                foreach (var project in projects)
                {
                    // Base associations for adminUser and usualUser
                    projectUsers.Add(ProjectUser.New(ProjectUserId.New(), project.Id, adminUser.Id, projectManagerRole.Id));
                    projectUsers.Add(ProjectUser.New(ProjectUserId.New(), project.Id, usualUser.Id, projectSponsorRole.Id));
                    projectUsers.Add(ProjectUser.New(ProjectUserId.New(), project.Id, usualUser.Id, teamMemberRole.Id));

                    // Random associations for additional users (1–2 roles per project per user, ensuring coverage)
                    var additionalUsers = new[] { manager1, sponsor1, analyst1, team1, user3 };
                    foreach (var user in additionalUsers)
                    {
                        var roleCount = random.Next(1, 3); // 1 or 2 roles per user per project
                        var availableRoles = new[] { projectManagerRole, projectSponsorRole, businessAnalystRole, teamMemberRole }
                            .Where(r => r.Name != (user == manager1 ? "Project manager" : 
                                           user == sponsor1 ? "Project sponsor" : 
                                           user == analyst1 ? "Business analyst" : 
                                           user == team1 ? "Project team member" : null))
                            .ToArray();
                        for (int i = 0; i < roleCount && availableRoles.Length > 0; i++)
                        {
                            var role = availableRoles[random.Next(availableRoles.Length)];
                            projectUsers.Add(ProjectUser.New(ProjectUserId.New(), project.Id, user.Id, role.Id));
                            availableRoles = availableRoles.Where(r => r.Id != role.Id).ToArray(); // Avoid duplicates
                        }
                    }
                }

                await context.ProjectUsers.AddRangeAsync(projectUsers);
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
                var projectUsers = await context.ProjectUsers
                    .Include(pu => pu.User)
                    .Include(pu => pu.Project)
                    .ToListAsync();
                var users = await context.Users.ToListAsync();

                if (!projects.Any() || !projectTasks.Any() || !projectUsers.Any() || !users.Any())
                {
                    throw new Exception(
                        "Projects, project tasks, project users, and users must be seeded before adding time entries.");
                }

                var timeEntries = new List<TimeEntry>();
                var random = new Random();
                var usedTimes = new HashSet<(Guid UserId, DateTime StartTime, DateTime EndTime)>();

                foreach (var projectUser in projectUsers)
                {
                    var user = projectUser.User;
                    var project = projectUser.Project;
                    var projectTasksForProject = projectTasks.Where(pt => pt.ProjectId == project.Id && pt.CreatorId == projectUser.UserId).ToList();
                    if (projectTasksForProject.Count == 0)
                    {
                        continue;
                    }
                    var maxEntries = 5; // 5 entries per user per project, total ~600–700

                    for (int i = 1; i <= maxEntries; i++)
                    {
                        bool timeOverlap;
                        DateTime startTime, endTime;
                        ProjectTask? selectedProjectTask = null;
                        do
                        {
                            startTime = DateTime.UtcNow.AddDays(random.Next(-60, 0)).AddHours(random.Next(8, 18)); // Work hours: 8:00–18:00
                            endTime = startTime.AddHours(random.Next(1, 4)); // Duration: 1–4 hours

                            timeOverlap = usedTimes.Any(t =>
                                t.UserId == user.Id &&
                                (
                                    (startTime < t.EndTime && (endTime > t.StartTime)) ||
                                    (t.StartTime < endTime && (t.EndTime > startTime))
                                ));

                            // Randomly select a ProjectTask for this project if available
                            if (projectTasksForProject.Any())
                            {
                                selectedProjectTask = projectTasksForProject[random.Next(projectTasksForProject.Count)];
                            }
                        } while (timeOverlap);

                        usedTimes.Add((user.Id, startTime, endTime));

                        timeEntries.Add(TimeEntry.New(
                            TimeEntryId.New(),
                            selectedProjectTask != null
                                ? $"Work on {selectedProjectTask.Name} - Entry {i} for {user.UserName}"
                                : $"Work on {project.Name} - Entry {i} for {user.UserName}",
                            startTime,
                            endTime,
                            (int)endTime.Subtract(startTime).TotalMinutes,
                            user.Id,
                            project.Id,
                            selectedProjectTask.Id
                        ));
                    }
                }

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
            await app.SeedTimeEntries();
        }

        private static string RandomColor()
        {
            var random = new Random();
            byte[] bytes = new byte[3];
            random.NextBytes(bytes);
            return $"{bytes[0]:X2}{bytes[1]:X2}{bytes[2]:X2}";
        }
    }
}