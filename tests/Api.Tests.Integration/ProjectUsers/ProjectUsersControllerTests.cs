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

namespace Api.Tests.Integration.ProjectUsers;

public class ProjectUsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _creatorRole;
    private readonly ProjectUser _existingProjectUser;
    private readonly Project _existingProject;
    private readonly Project _existingProject2;
    private readonly User _mainUser;

    public ProjectUsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser2;
        _creatorRole = RolesData.Creator1Role;
        _existingProject2 = ProjectsData.ExistingProject2(_mainUser.Id, _mainUser.Id);
        _existingProject = ProjectsData.ExistingProjectForProjectUser(_mainUser.Id, _mainUser.Id);
        _existingProjectUser = ProjectUsersData.ExistingProjectUser(_existingProject.Id, _mainUser.Id, _creatorRole.Id);
    }
[Fact]
    public async Task ShouldCreateProjectUser()
    {
        // Arrange
        var projectId = _existingProject2.Id.Value;
        var userId = _mainUser.Id;
        var roleId = _creatorRole.Id;
        var request = new ProjectUserCreateDto
        (
            ProjectId: projectId,
            UserId: userId,
            RoleId:roleId
        );
        
        // Act
        var response = await Client.PostAsJsonAsync("projects/add-user-to-project", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseProjectUser = await response.ToResponseModel<ProjectUserDto>();
        responseProjectUser.Id.Should().NotBeEmpty();
        responseProjectUser.ProjectId.Should().Be(request.ProjectId);
        responseProjectUser.UserId.Should().Be(request.UserId);
        responseProjectUser.RoleId.Should().Be(request.RoleId);
    }

    [Fact]
    public async Task ShouldNotCreateProjectUserBecauseAlreadyExists()
    {
        // Arrange
        var projectId = _existingProject.Id.Value;
        var userId = _mainUser.Id;
        var request = new ProjectUserCreateDto
        (
            ProjectId: projectId,
            UserId: userId,
            RoleId: _creatorRole.Id
        );
        
        // Act
        var response = await Client.PostAsJsonAsync("projects/add-user-to-project", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task ShouldNotCreateProjectUserBecauseProjectNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = _mainUser.Id;
        var request = new ProjectUserCreateDto
        (
            ProjectId: projectId,
            UserId: userId,
            RoleId: _creatorRole.Id
        );
        
        // Act
        var response = await Client.PostAsJsonAsync("projects/add-user-to-project", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotCreateProjectUserBecauseUserNotFound()
    {
        // Arrange
        var projectId = _existingProject2.Id.Value;
        var userId = Guid.NewGuid();
        var request = new ProjectUserCreateDto
        (
            ProjectId: projectId,
            UserId: userId,
            RoleId: _creatorRole.Id
        );
        
        // Act
        var response = await Client.PostAsJsonAsync("projects/add-user-to-project", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotCreateProjectUserBecauseRoleNotFound()
    {
        // Arrange
        var projectId = _existingProject2.Id.Value;
        var userId = _mainUser.Id;
        var request = new ProjectUserCreateDto
        (
            ProjectId: projectId,
            UserId: userId,
            RoleId: Guid.NewGuid()
        );
        
        // Act
        var response = await Client.PostAsJsonAsync("projects/add-user-to-project", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldDeleteProjectUser()
    {
        // Arrange
        var projectUserId = _existingProjectUser.Id;

        // Act
        var response = await Client.DeleteAsync($"projects/remove-user-from-project/{projectUserId}");
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseProjectUser = await response.ToResponseModel<ProjectUserDto>();
        responseProjectUser.Id.Should().Be(projectUserId.Value);
        
        var dbProjectUser = await Context.ProjectUsers.FirstOrDefaultAsync(x => x.Id == projectUserId);
        dbProjectUser.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteProjectUserBecauseIdNotFound()
    {
        // Arrange
        var projectUserId = Guid.NewGuid(); 

        // Act
        var response = await Client.DeleteAsync($"projects/remove-user-from-project/{projectUserId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await Context.Projects.AddAsync(_existingProject);
        await Context.Projects.AddAsync(_existingProject2);
        await Context.Roles.AddAsync(_creatorRole);
        await Context.ProjectUsers.AddAsync(_existingProjectUser);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.ProjectUsers.RemoveRange(Context.ProjectUsers);
        Context.Projects.RemoveRange(Context.Projects);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}