using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.Users;
using Domain.Models.UsersTasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.UserTasks;

public class UserTasksControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Project _existingProject;
    private readonly ProjectTask _existingProjectTask;
    private readonly ProjectTask _existingProjectTask2;
    private readonly User _mainUser;
    private readonly UserTask _existingUserTask;

    public UserTasksControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser2;
        _existingProject = ProjectsData.ExistingProject2(_mainUser.Id, _mainUser.Id);
        _existingProjectTask = ProjectTasksData.ExistingProjectTask(_existingProject.Id);
        _existingProjectTask2 = ProjectTasksData.ExistingProjectTask2(_existingProject.Id);
        _existingUserTask = UserTasksData.ExistingUserTask(_existingProjectTask2.Id, _mainUser.Id);
    }

    [Fact]
    public async Task ShouldAddUserToProjectTask()
    {
        // Arrange
        var request = new UserTaskCreateDto
        (
            _existingProjectTask.Id.Value,
            _mainUser.Id
        );

        // Act
        var response = await Client.PostAsJsonAsync("project-tasks/add-user-to-project-task", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseUserTask = await response.ToResponseModel<UserTaskDto>();
        responseUserTask.Id.Should().NotBeEmpty();
        responseUserTask.ProjectTaskId.Should().Be(request.ProjectTaskId);
        responseUserTask.UserId.Should().Be(request.UserId);
    }
    
    [Fact]
    public async Task ShouldNotAddUserToProjectTaskBecauseProjectTaskNotFound()
    {
        // Arrange
        var request = new UserTaskCreateDto
        (
            Guid.NewGuid(),
            _mainUser.Id
        );

        // Act
        var response = await Client.PostAsJsonAsync("project-tasks/add-user-to-project-task", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotAddUserToProjectTaskBecauseUserNotFound()
    {
        // Arrange
        var request = new UserTaskCreateDto
        (
            _existingProjectTask.Id.Value,
            Guid.NewGuid()
        );

        // Act
        var response = await Client.PostAsJsonAsync("project-tasks/add-user-to-project-task", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotAddUserToProjectTaskBecauseAlreadyExists()
    {
        // Arrange
        var request = new UserTaskCreateDto
        (
            _existingProjectTask2.Id.Value,
            _mainUser.Id
        );

        // Act
        var response = await Client.PostAsJsonAsync("project-tasks/add-user-to-project-task", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict); 
    }
    
    [Fact]
    public async Task ShouldRemoveUserFromProjectTask()
    {
        var existingUserTaskId = _existingUserTask.Id;
        // Act
        var response = await Client.DeleteAsync($"project-tasks/remove-user-from-project-task/{existingUserTaskId.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseUserTask = await response.ToResponseModel<UserTaskDto>();
        responseUserTask.Id.Should().Be(_existingUserTask.Id.Value);

        var dbProjectUser = await Context.UserTasks.FirstOrDefaultAsync(x => x.Id == existingUserTaskId);
        dbProjectUser.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotRemoveUserFromProjectTaskBecauseNotFound()
    {
        // Act
        var response = await Client.DeleteAsync($"project-tasks/remove-user-from-project-task/{Guid.NewGuid()}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public async Task InitializeAsync()
        {
            await Context.Users.AddAsync(_mainUser);
            await SaveChangesAsync();
            await Context.Projects.AddAsync(_existingProject);
            await Context.ProjectTasks.AddAsync(_existingProjectTask);
            await Context.ProjectTasks.AddAsync(_existingProjectTask2);
            await SaveChangesAsync();
            await Context.UserTasks.AddAsync(_existingUserTask);
            await SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            Context.UserTasks.RemoveRange(Context.UserTasks);
            Context.ProjectTasks.RemoveRange(Context.ProjectTasks);
            Context.Projects.RemoveRange(Context.Projects);
            Context.Users.RemoveRange(Context.Users);
            await SaveChangesAsync();
        }
    }