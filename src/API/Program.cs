using System.Text;
using Api.Modules;
using Application;
using Application.Common.Middleware;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Infrastructure.Authentication;
using Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Configure application services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddApplication();
//Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectManger", Version = "v1" });
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true
        };
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/users/login"; // Redirect path for non-API unauthenticated requests
        options.Events.OnRedirectToLogin = context =>
        {
            // Return 401 for API requests instead of redirecting
            if (context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/users"))
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    })
    .AddFacebook(options =>
    {
        options.ClientId = "657579603610519";
        options.ClientSecret = "ef7a736a4409fe0d4f4e63e1d20e3f44";
        options.CallbackPath = "/signin-facebook";
        options.Scope.Add("email");
        options.Fields.Add("name");
        options.Fields.Add("email");
        options.SaveTokens = true;

        options.Events.OnRemoteFailure = context =>
        {
            Console.WriteLine($"Remote failure: {context.Failure?.Message} - {context.Failure?.StackTrace}");
            context.HandleResponse();
            // Redirect to frontend login with error
            var error = context.Failure?.Message ?? "Login cancelled or failed";
            var redirectUrl = $"http://localhost:5173/login?error={Uri.EscapeDataString("access_denied")}&error_description={Uri.EscapeDataString(error)}";
            context.Response.Redirect(redirectUrl);
            return Task.CompletedTask;
        };

        options.Events.OnTicketReceived = context =>
        {
            Console.WriteLine("Ticket received from Facebook");
            return Task.CompletedTask;
        };

        options.Events.OnCreatingTicket = async context =>
        {
            var userInfo = context.Identity;
            Console.WriteLine($"User authenticated: {userInfo.Name}");
        };
    });

// CORS policy setup
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
        options => options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
await app.InitializeDb();
app.MapControllers();


var isTesting = builder.Configuration.GetValue<bool>("IsTesting");

if (!isTesting)
{
    app.UseHangfireDashboard();
    RecurringJob.AddOrUpdate<TimeEntryNotificationService>(
        "schedule-time-entry-notifications",
        service => service.ScheduleTimeEntryNotifications(),
        Cron.Daily()
    );
}



app.Run();

public partial class Program;