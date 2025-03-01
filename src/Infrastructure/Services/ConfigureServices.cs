using Application.Common.Interfaces.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        
        AddNotifications(services);
    }
    
    private static void AddNotifications(IServiceCollection services)
    {
        services.AddControllersWithViews()
            .AddRazorRuntimeCompilation();

        
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new InfrastructureViewLocationExpander());
        });

        services.AddSingleton<ITempDataProvider, CookieTempDataProvider>(); 
        services.AddSingleton<ICompositeViewEngine, CompositeViewEngine>(); 

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITimeEntryNotificationService, TimeEntryNotificationService>();
    }
}