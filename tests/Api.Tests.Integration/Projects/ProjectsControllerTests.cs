using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Projects;
using Domain.Models.Roles;
using Domain.Models.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Projects;

public class ProjectsControllerTests : BaseIntegrationTest, IAsyncLifetime
{    
    private readonly User _mainUser;
    private readonly Project _mainProject;

    public ProjectsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser;
        _mainProject = ProjectsData.NewProject(_mainUser.Id, _mainUser.Id);
    }
    
    [Fact]
    public async Task ShouldCreateProject()
    {
        // Arrange
        var request = new ProjectDto
        (
            Id: null,
            Name: "TestProject",
            Description: "TestProjectDescription",
            ColorHex: "#ffffff",
            CreatedAt: DateTime.UtcNow,
            CreatorId: _mainUser.Id,
            ClientId: _mainUser.Id
            
        );

        // Act
        var response = await Client.PostAsJsonAsync("projects/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdProject = await response.ToResponseModel<Project>();
        createdProject.Id.Value.Should().NotBeEmpty();
        createdProject.Name.Should().Be(request.Name);
        createdProject.Description.Should().Be(request.Description);
        createdProject.ColorHex.Should().Be(request.ColorHex);
        createdProject.CreatedAt.Should().Be(request.CreatedAt);
        createdProject.CreatorId.Should().Be(request.CreatorId);
        createdProject.ClientId.Should().Be(request.ClientId);
    }
    
    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Projects.RemoveRange(Context.Projects);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}