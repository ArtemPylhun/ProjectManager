using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.ProjectTasks;

public class ProjectTasksControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Project _existingProject;
    private readonly User _mainUser;
    private readonly ProjectTask _newProjectTask;
    private readonly ProjectTask _existingProjectTask;
    private readonly ProjectTask _existingProjectTask2;

    public ProjectTasksControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser;
        _existingProject = ProjectsData.ExistingProject(_mainUser.Id, _mainUser.Id);
        _newProjectTask = ProjectTasksData.NewProjectTask(_existingProject.Id);
        _existingProjectTask = ProjectTasksData.ExistingProjectTask(_existingProject.Id);
        _existingProjectTask2 = ProjectTasksData.ExistingProjectTask2(_existingProject.Id);
    }

    [Fact]
    public async Task ShouldCreateProjectTask()
    {
        //Arrange
        var request = new ProjectTaskCreateDto(_existingProject.Id, _newProjectTask.Name, 1);
        //Act
        var response = await Client.PostAsJsonAsync("project-tasks/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldNotCreateProjectTaskBecauseProjectNotFound()
    {
        //Arrange
        var request = new ProjectTaskCreateDto(ProjectId.New(), "NewProjectTask", 1);
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
        var request = new ProjectTaskCreateDto(_existingProject.Id, _existingProjectTask.Name, 1);
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
        var request = new ProjectTaskUpdateDto(_existingProjectTask.Id, _existingProject.Id, "NewProjectTask", 1);
        //Act
        var response = await Client.PutAsJsonAsync("project-tasks/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldNotUpdateProjectTaskBecauseIdNotFound()
    {
        //Arrange
        var request = new ProjectTaskUpdateDto(ProjectTaskId.New(), _existingProject.Id, "NewProjectTask", 1);
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
        var request = new ProjectTaskUpdateDto(_existingProjectTask.Id, ProjectId.New(), "NewProjectTask", 1);
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
        var request = new ProjectTaskUpdateDto(_existingProjectTask.Id, _existingProject.Id, _existingProjectTask2.Name, 1);
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
        var projectTaskId = _existingProjectTask.Id;
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
        await Context.Projects.AddAsync(_existingProject);
        await Context.ProjectTasks.AddAsync(_existingProjectTask);
        await Context.ProjectTasks.AddAsync(_existingProjectTask2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Projects.RemoveRange(Context.Projects);
        Context.ProjectTasks.RemoveRange(Context.ProjectTasks);
        await SaveChangesAsync();
    }
}