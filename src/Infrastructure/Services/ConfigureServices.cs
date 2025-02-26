using Application.Common.Interfaces.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        var isTesting = bool.Parse(configuration.GetSection("IsTesting").Value!);
        if (!isTesting)
        {
            var connectionString = configuration.GetConnectionString("Default");
            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(connectionString));
            services.AddHangfireServer();
        }
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITimeEntryNotificationService, TimeEntryNotificationService>();
    }
}