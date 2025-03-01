using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Projects;
using Domain.Models.ProjectUsers;
using Domain.Models.Roles;
using Domain.Models.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Projects;

public class ProjectsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly Project _existingProject;
    private readonly Project _existingProject2;
    private readonly Role _projectRole;

    public ProjectsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser;
        _existingProject = ProjectsData.ExistingProject(_mainUser.Id, _mainUser.Id);
        _existingProject2 = ProjectsData.ExistingProject2(_mainUser.Id, _mainUser.Id);
        _projectRole = RolesData.ProjectRole;
    }

    [Fact]
    public async Task ShouldCreateProject()
    {
        // Arrange
        var projectName = "TestProject";
        var projectDesc = "TestProjectDescription";
        var colorHex = "#ffffff";
        var request = new ProjectCreateDto
        (
            Name: projectName,
            Description: projectDesc,
            ColorHex: colorHex,
            CreatedAt: DateTime.UtcNow,
            CreatorId: _mainUser.Id,
            ClientId: _mainUser.Id
        );

        // Act
        var response = await Client.PostAsJsonAsync("projects/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdProject = await response.ToResponseModel<ProjectDto>();
        createdProject.Id.Should().NotBeEmpty();
        createdProject.Name.Should().Be(request.Name);
        createdProject.Description.Should().Be(request.Description);
        createdProject.ColorHex.Should().Be(request.ColorHex);
        createdProject.CreatedAt.Should().BeCloseTo(request.CreatedAt, precision: TimeSpan.FromMilliseconds(1500));
        createdProject.CreatorId.Should().Be(request.CreatorId);
        createdProject.ClientId.Should().Be(request.ClientId);
    }

    [Fact]
    public async Task ShouldNotCreateProjectBecauseClientNotFound()
    {
        // Arrange
        var projectName = "TestProject";
        var projectDesc = "TestProjectDescription";
        var colorHex = "#ffffff";
        var request = new ProjectCreateDto
        (
            Name: projectName,
            Description: projectDesc,
            ColorHex: colorHex,
            CreatedAt: DateTime.UtcNow,
            CreatorId: _mainUser.Id,
            ClientId: Guid.NewGuid()
        );

        // Act
        var response = await Client.PostAsJsonAsync("projects/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateProjectBecauseCreatorNotFound()
    {
        // Arrange
        var projectName = "TestProject";
        var projectDescription = "TestProjectDescription";
        var colorHex = "#ffffff";
        var request = new ProjectCreateDto
        (
            Name: projectName,
            Description: projectDescription,
            ColorHex: colorHex,
            CreatedAt: DateTime.UtcNow,
            CreatorId: Guid.NewGuid(),
            ClientId: _mainUser.Id
        );

        // Act
        var response = await Client.PostAsJsonAsync("projects/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateProjectBecauseAlreadyExists()
    {
        // Arrange
        var projectDesc = "TestProjectDescription";
        var colorHex = "#ffffff";
        var request = new ProjectCreateDto
        (
            Name: _existingProject.Name,
            Description: projectDesc,
            ColorHex: colorHex,
            CreatedAt: DateTime.UtcNow,
            CreatorId: _mainUser.Id,
            ClientId: _mainUser.Id
        );

        // Act
        var response = await Client.PostAsJsonAsync("projects/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateProject()
    {
        //Arrange
        var projectName = "TestProject2";
        var projectDesc = "TestProjectDescription2";
        var colorHex = "#8e574b";
        var request = new ProjectUpdateDto(
            Id: _existingProject.Id.Value,
            Name: projectName,
            Description: projectDesc,
            ColorHex: colorHex,
            ClientId: _mainUser.Id);
        //Act
        var response = await Client.PutAsJsonAsync("projects/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdProject = await response.ToResponseModel<ProjectDto>();
        createdProject.Id.Should().Be(_existingProject.Id.Value);

        var dbProject = await Context.Projects.FirstOrDefaultAsync(x => x.Id == _existingProject.Id);

        dbProject.Should().NotBeNull();
        dbProject.Id.Value.Should().Be(_existingProject.Id.Value);
        dbProject.Name.Should().Be(request.Name);
        dbProject.Description.Should().Be(request.Description);
        dbProject.ColorHex.Should().Be(request.ColorHex);
        dbProject.ClientId.Should().Be(request.ClientId);
    }

    [Fact]
    public async Task ShouldNotUpdateProjectBecauseIdNotFound()
    {
        //Arrange
        var projectName = "TestProject2";
        var projectDesc = "TestProjectDescription2";
        var colorHex = "#8e574b";
        var request = new ProjectUpdateDto(
            Id: Guid.NewGuid(),
            Name: projectName,
            Description: projectDesc,
            ColorHex: colorHex,
            ClientId: _mainUser.Id);
        //Act
        var response = await Client.PutAsJsonAsync("projects/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateProjectBecauseSuchProjectAlreadyExists()
    {
        //Arrange
        var projectDesc = "TestProjectDescription2";
        var colorHex = "#8e574b";
        var request = new ProjectUpdateDto(
            Id: _existingProject.Id.Value,
            Name: _existingProject2.Name,
            Description: projectDesc,
            ColorHex: colorHex,
            ClientId: _mainUser.Id);
        //Act
        var response = await Client.PutAsJsonAsync("projects/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldDeleteProject()
    {
        // Arrange
        var projectId = _existingProject2.Id;

        // Act
        var response = await Client.DeleteAsync($"projects/delete/{projectId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseProject = await response.ToResponseModel<ProjectDto>();
        responseProject.Id.Should().Be(_existingProject2.Id.Value);

        var dbproject = await Context.Projects.FirstOrDefaultAsync(x => x.Id == _existingProject2.Id);
        dbproject.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteProjectBecauseIdNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"projects/delete/{projectId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await SaveChangesAsync();
        await Context.Roles.AddAsync(_projectRole);
        await Context.Projects.AddAsync(_existingProject);
        await Context.Projects.AddAsync(_existingProject2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.ProjectTasks.RemoveRange(Context.ProjectTasks);
        Context.Projects.RemoveRange(Context.Projects);
        Context.UserRoles.RemoveRange(Context.UserRoles);
        Context.Roles.RemoveRange(Context.Roles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}