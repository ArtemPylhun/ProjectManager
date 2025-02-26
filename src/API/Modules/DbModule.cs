using Infrastructure.Persistence;

namespace Api.Modules;

public static class DbModule
{
    public static async Task InitializeDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitializeAsync();
        
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        if (bool.Parse(config["AllowSeeder"]!))
        {
            await app.SeedAll();
        }
    }
}