
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.Roles;
using Domain.Models.Users;
using Infrastructure.Authentication;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistence
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceBuild = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Default"));
        dataSourceBuild.EnableDynamicJson();
        var dataSource = dataSourceBuild.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));

        services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
        services.AddScoped<IUserQueries>(provider => provider.GetRequiredService<UserRepository>());
        
        services.AddScoped<RoleRepository>();
        services.AddScoped<IRoleRepository>(provider => provider.GetRequiredService<RoleRepository>());
        services.AddScoped<IRoleQueries>(provider => provider.GetRequiredService<RoleRepository>());

        services.AddScoped<EmailNotificationRepository>();
        services.AddScoped<IEmailNotificationRepository>(provider => provider.GetRequiredService<EmailNotificationRepository>());
        services.AddScoped<IEmailNotificationQueries>(provider => provider.GetRequiredService<EmailNotificationRepository>());

        services.AddScoped<ProjectRepository>();
        services.AddScoped<IProjectRepository>(provider => provider.GetRequiredService<ProjectRepository>());
        services.AddScoped<IProjectQueries>(provider => provider.GetRequiredService<ProjectRepository>());

        services.AddScoped<ProjectTaskRepository>();
        services.AddScoped<IProjectTaskRepository>(provider => provider.GetRequiredService<ProjectTaskRepository>());
        services.AddScoped<IProjectTaskQueries>(provider => provider.GetRequiredService<ProjectTaskRepository>());

        services.AddScoped<ProjectUserRepository>();
        services.AddScoped<IProjectUserRepository>(provider => provider.GetRequiredService<ProjectUserRepository>());
        services.AddScoped<IProjectUserQueries>(provider => provider.GetRequiredService<ProjectUserRepository>());
        
        services.AddScoped<TimeEntryRepository>();
        services.AddScoped<ITimeEntryRepository>(provider => provider.GetRequiredService<TimeEntryRepository>());
        services.AddScoped<ITimeEntryQueries>(provider => provider.GetRequiredService<TimeEntryRepository>());

        services.AddScoped<UserTaskRepository>();
        services.AddScoped<IUserTaskRepository>(provider => provider.GetRequiredService<UserTaskRepository>());
        services.AddScoped<IUserTaskQueries>(provider => provider.GetRequiredService<UserTaskRepository>());
        
        services.AddScoped<JwtProvider>();
        services.AddScoped<IJwtProvider>(provider => provider.GetRequiredService<JwtProvider>());
    }
}