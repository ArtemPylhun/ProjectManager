using System.Text;
using Api.Modules;
using Application;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Infrastructure.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var isTesting = builder.Environment.EnvironmentName == "Testing" ||
                builder.Configuration.GetValue<bool>("IsTesting");

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
    });

/*
if (!isTesting)
{
    //Adding Hangfire
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Default")));
            
    builder.Services.AddHangfireServer();
}
*/

// CORS policy setup
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
        options => options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
var app = builder.Build();
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
/*
if (!isTesting)
{
    app.UseHangfireDashboard();
    app.MapHangfireDashboard();
}
*/



app.Run();

public partial class Program;