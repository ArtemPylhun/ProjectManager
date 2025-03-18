using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Application.Common.Interfaces;
using Domain.Models.Roles;
using Domain.Models.Users;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tests.Data;
using Xunit;

namespace Tests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory>
{
    protected readonly ApplicationDbContext Context;
    protected readonly HttpClient Client;
    protected readonly UserManager<User> UserManager;
    protected readonly RoleManager<Role> RoleManager;
    protected readonly IJwtProvider JwtProvider;
    private readonly User _adminUser = UsersData.AdminUser;
    private readonly Role _adminRole = RolesData.AdminRole;

    protected BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        var scope = factory.Services.CreateScope();

        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        JwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        Client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: "TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "TestScheme", _ => { });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

        SetAdminAuthorizationHeaderAsync().GetAwaiter().GetResult();
    }

    protected async Task SetAdminAuthorizationHeaderAsync()
    {
        await SaveAdminAsync();
        SetCustomAuthorizationHeader(JwtProvider.GenerateToken(_adminUser, new List<string>() {_adminRole.Name} ));
    }

    private async Task SaveAdminAsync()
    {
        if (await RoleManager.FindByNameAsync(_adminRole.Name!) == null)
        {
            await RoleManager.CreateAsync(_adminRole);
        }

        var user = await UserManager.FindByEmailAsync(_adminUser.Email);
        if (user == null)
        {
            var createUserResult = await UserManager.CreateAsync(_adminUser, "AdminPass123!");
            if (!createUserResult.Succeeded)
            {
                throw new Exception($"Failed to create admin user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
            }
        }

        if (!await UserManager.IsInRoleAsync(_adminUser, _adminRole.Name!))
        {
            var addToRoleResult = await UserManager.AddToRoleAsync(_adminUser, _adminRole.Name!);
            if (!addToRoleResult.Succeeded)
            {
                throw new Exception($"Failed to add admin role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
            }
        }
    }

    protected void SetCustomAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<int> SaveChangesAsync()
    {
        var result = await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        return result;
    }
}

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Role, "Admin"), new Claim("userId", "admin") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}