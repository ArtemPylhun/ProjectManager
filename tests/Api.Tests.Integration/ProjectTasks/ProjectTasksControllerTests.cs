using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.ProjectTasks;

public class ProjectTasksControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Project _existingProject;
    private readonly User _mainUser;
    private readonly User _mainUser2;
    private readonly ProjectTask _newProjectTask;
    private readonly ProjectTask _existingProjectTask;
    private readonly ProjectTask _existingProjectTask2;

    public ProjectTasksControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser;
        _mainUser2 = UsersData.MainUser2;
        _existingProject = ProjectsData.ExistingProject(_mainUser.Id, _mainUser.Id);
        _newProjectTask = ProjectTasksData.NewProjectTask(_existingProject.Id, _mainUser.Id);
        _existingProjectTask = ProjectTasksData.ExistingProjectTask(_existingProject.Id, _mainUser2.Id);
        _existingProjectTask2 = ProjectTasksData.ExistingProjectTask2(_existingProject.Id, _mainUser2.Id);
    }

    [Fact]
    public async Task ShouldCreateProjectTask()
    {
        //Arrange
        var request = new ProjectTaskCreateDto(_existingProject.Id.Value, _mainUser.Id, _newProjectTask.Name, 120,
            "Description for task3233212313");
        //Act
        var response = await Client.PostAsJsonAsync("project-tasks/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdProjectTask = await response.ToResponseModel<ProjectTaskDto>();
        createdProjectTask.Id.Should().NotBeEmpty();
        createdProjectTask.Name.Should().Be(request.Name);
        createdProjectTask.EstimatedTime.Should().Be(request.EstimatedTime);
        createdProjectTask.ProjectId.Should().Be(request.ProjectId);
    }

    [Fact]
    public async Task ShouldNotCreateProjectTaskBecauseProjectNotFound()
    {
        //Arrange
        var request = new ProjectTaskCreateDto(ProjectId.New().Value, _mainUser.Id, "NewProjectTask", 1,
            "Description for task");
        //Act
        var response = await Client.PostAsJsonAsync("project-tasks/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateProjectTaskBecauseUserNotFound()
    {
        //Arrange
        var request = new ProjectTaskCreateDto(_existingProject.Id.Value, Guid.NewGuid(), "NewProjectTask", 1,
            "Description for task");
        //Act
        var response = await Client.PostAsJsonAsync("project-tasks/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateProjectTaskBecauseProjectTaskAlreadyExists()
    {
        //Arrange
        var request = new ProjectTaskCreateDto(_existingProject.Id.Value, _mainUser.Id, _existingProjectTask.Name, 1,
            "Description for task");
        //Act
        var response = await Client.PostAsJsonAsync("project-tasks/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldUpdateProjectTask()
    {
        //Arrange
        var request = new ProjectTaskUpdateDto(_existingProjectTask.Id.Value, _existingProject.Id.Value,
            "NewProjectTask", 1, "Description for task", ProjectTask.ProjectTaskStatuses.New);
        //Act
        var response = await Client.PutAsJsonAsync("project-tasks/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedProjectTask = await response.ToResponseModel<ProjectTaskDto>();
        updatedProjectTask.Id.Should().Be(_existingProjectTask.Id.Value);

        var dbProjectTask = await Context.ProjectTasks.FirstOrDefaultAsync(x => x.Id == _existingProjectTask.Id);

        dbProjectTask.Should().NotBeNull();
        dbProjectTask.Id.Value.Should().Be(_existingProjectTask.Id.Value);
        dbProjectTask.Name.Should().Be(request.Name);
        dbProjectTask.EstimatedTime.Should().Be(request.EstimatedTime);
        dbProjectTask.ProjectId.Value.Should().Be(request.ProjectId);
    }

    [Fact]
    public async Task ShouldNotUpdateProjectTaskBecauseIdNotFound()
    {
        //Arrange
        var request = new ProjectTaskUpdateDto(ProjectTaskId.New().Value, _existingProject.Id.Value, "NewProjectTask",
            1, "Description for task", ProjectTask.ProjectTaskStatuses.New);
        //Act
        var response = await Client.PutAsJsonAsync("project-tasks/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateProjectTaskBecauseProjectNotFound()
    {
        //Arrange
        var request = new ProjectTaskUpdateDto(_existingProjectTask.Id.Value, ProjectId.New().Value, "NewProjectTask",
            1, "Description for task", ProjectTask.ProjectTaskStatuses.New);
        //Act
        var response = await Client.PutAsJsonAsync("project-tasks/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateProjectTaskBecauseProjectTaskAlreadyExists()
    {
        //Arrange
        var request = new ProjectTaskUpdateDto(_existingProjectTask.Id.Value, _existingProject.Id.Value,
            _existingProjectTask2.Name, 1, "Description for task", ProjectTask.ProjectTaskStatuses.New);
        //Act
        var response = await Client.PutAsJsonAsync("project-tasks/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldDeleteProjectTask()
    {
        //Arrange
        var projectTaskId = _existingProjectTask.Id.Value;
        //Act
        var response = await Client.DeleteAsync($"project-tasks/delete/{projectTaskId}");
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldNotDeleteProjectTaskBecauseIdNotFound()
    {
        //Arrange
        var projectTaskId = ProjectTaskId.New();
        //Act
        var response = await Client.DeleteAsync($"project-tasks/delete/{projectTaskId}");
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await Context.Users.AddAsync(_mainUser2);
        await SaveChangesAsync();
        await Context.Projects.AddAsync(_existingProject);
        await SaveChangesAsync();
        await Context.ProjectTasks.AddAsync(_existingProjectTask);
        await Context.ProjectTasks.AddAsync(_existingProjectTask2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.ProjectTasks.RemoveRange(Context.ProjectTasks);
        Context.Projects.RemoveRange(Context.Projects);
        Context.UserRoles.RemoveRange(Context.UserRoles);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}