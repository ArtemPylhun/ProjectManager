using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;
using Domain.Models.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.TimeEntries;

public class TimeEntriesControllerTests: BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser2;
    private readonly Project _project;
    private readonly ProjectTask _projectTask;
    private readonly TimeEntry _timeEntry;

    public TimeEntriesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser2 = UsersData.MainUser2;
        _project = ProjectsData.ExistingProject2(_mainUser2.Id, _mainUser2.Id);
        _projectTask = ProjectTasksData.ExistingProjectTask(_project.Id);
        _timeEntry = TimeEntriesData.NewTimeEntry(_mainUser2.Id, _project.Id, _projectTask.Id);
    }

    [Fact]
    public async Task ShouldCreateTimeEntry()
    {
        //Arrange
        var request = new TimeEntryCreateDto(
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            _timeEntry.ProjectId,
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PostAsJsonAsync("time-entries/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var createdTimeEntry = await response.ToResponseModel<TimeEntryDto>();
        createdTimeEntry.Id.Value.Should().NotBeEmpty();
        createdTimeEntry.Description.Should().Be(request.Description);
        createdTimeEntry.StartTime.Should().BeCloseTo(request.StartTime, precision: TimeSpan.FromMilliseconds(500));
        createdTimeEntry.EndTime.Should().BeCloseTo(request.EndTime, precision: TimeSpan.FromMilliseconds(500));
        createdTimeEntry.Minutes.Should().Be(request.Minutes);
        createdTimeEntry.UserId.Should().Be(request.UserId);
        createdTimeEntry.ProjectId.Should().Be(request.ProjectId);
        createdTimeEntry.ProjectTaskId.Should().Be(request.ProjectTaskId);
        
    }
    
    [Fact]
    public async Task ShouldNotCreateTimeEntryBecauseProjectTaskNotFound()
    {
        //Arrange
        var request = new TimeEntryCreateDto(
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            _timeEntry.ProjectId,
            ProjectTaskId.New());
        //Act
        var response = await Client.PostAsJsonAsync("time-entries/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotCreateTimeEntryBecauseProjectNotFound()
    {
        //Arrange
        var request = new TimeEntryCreateDto(
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            ProjectId.New(),
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PostAsJsonAsync("time-entries/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotCreateTimeEntryBecauseUserNotFound()
    {
        //Arrange
        var request = new TimeEntryCreateDto(
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            Guid.NewGuid(),
            _timeEntry.ProjectId,
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PostAsJsonAsync("time-entries/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotCreateTimeEntryBecauseStartDateIsAfterEndDate()
    {
        //Arrange
        var request = new TimeEntryCreateDto(
            _timeEntry.Description,
            _timeEntry.EndDate,
            _timeEntry.StartDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            _timeEntry.ProjectId,
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PostAsJsonAsync("time-entries/create", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        
    }

    [Fact]
    public async Task ShouldDeleteTimeEntry()
    {
        //Arrange
        var timeEntryId = _timeEntry.Id;
        //Act
        var response = await Client.DeleteAsync($"time-entries/delete/{timeEntryId}");
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var responseTimeEntry = await response.ToResponseModel<TimeEntryDto>();
        responseTimeEntry.Id.Value.Should().Be(_timeEntry.Id.Value);

        var dbTimeEntry = await Context.TimeEntries.FirstOrDefaultAsync(x => x.Id == _timeEntry.Id);
        dbTimeEntry.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteTimeEntryBecauseIdNotFound()
    {
        //Arrange
        var timeEntryId = TimeEntryId.New();
        //Act
        var response = await Client.DeleteAsync($"time-entries/delete/{timeEntryId}");
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldUpdateTimeEntry()
    {
        //Arrange
        var timeEntryId = _timeEntry.Id;
        var request = new TimeEntryUpdateDto(
            _timeEntry.Id,
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            _timeEntry.ProjectId,
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PutAsJsonAsync($"time-entries/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var createdTimeEntry = await response.ToResponseModel<TimeEntryDto>();
        createdTimeEntry.Id.Value.Should().Be(_timeEntry.Id.Value);

        var dbTimeEntry = await Context.TimeEntries.FirstOrDefaultAsync(x => x.Id == _timeEntry.Id);
        
        dbTimeEntry.Should().NotBeNull();
        dbTimeEntry.Id.Value.Should().Be(_timeEntry.Id.Value);
        dbTimeEntry.Description.Should().Be(request.Description);
        dbTimeEntry.StartDate.Should().BeCloseTo(request.StartTime, precision: TimeSpan.FromMilliseconds(500));
        dbTimeEntry.EndDate.Should().BeCloseTo(request.EndTime, precision: TimeSpan.FromMilliseconds(500));
        dbTimeEntry.Minutes.Should().Be(request.Minutes);
        dbTimeEntry.UserId.Should().Be(request.UserId);
        dbTimeEntry.ProjectId.Should().Be(request.ProjectId);
        dbTimeEntry.ProjectTaskId.Should().Be(request.ProjectTaskId);
    }
    
    [Fact]
    public async Task ShouldNotUpdateTimeEntryBecauseIdNotFound()
    {
        //Arrange
        var timeEntryId = TimeEntryId.New();
        var request = new TimeEntryUpdateDto(
            timeEntryId,
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            _timeEntry.ProjectId,
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PutAsJsonAsync($"time-entries/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotUpdateTimeEntryBecauseStartDateIsAfterEndDate()
    {
        //Arrange
        var timeEntryId = _timeEntry.Id;
        var request = new TimeEntryUpdateDto(
            _timeEntry.Id,
            _timeEntry.Description,
            _timeEntry.EndDate,
            _timeEntry.StartDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            _timeEntry.ProjectId,
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PutAsJsonAsync($"time-entries/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task ShouldNotUpdateTimeEntryBecauseUserNotFound()
    {
        //Arrange
        var timeEntryId = _timeEntry.Id;
        var request = new TimeEntryUpdateDto(
            _timeEntry.Id,
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            Guid.NewGuid(),
            _timeEntry.ProjectId,
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PutAsJsonAsync($"time-entries/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }   
    
    [Fact]
    public async Task ShouldNotUpdateTimeEntryBecauseProjectNotFound()
    {
        //Arrange
        var timeEntryId = _timeEntry.Id;
        var request = new TimeEntryUpdateDto(
            _timeEntry.Id,
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            ProjectId.New(),
            _timeEntry.ProjectTaskId);
        //Act
        var response = await Client.PutAsJsonAsync($"time-entries/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotUpdateTimeEntryBecauseProjectTaskNotFound()
    {
        //Arrange
        var request = new TimeEntryUpdateDto(
            _timeEntry.Id,
            _timeEntry.Description,
            _timeEntry.StartDate,
            _timeEntry.EndDate,
            _timeEntry.Minutes,
            _timeEntry.UserId,
            _timeEntry.ProjectId,
            ProjectTaskId.New());
        //Act
        var response = await Client.PutAsJsonAsync($"time-entries/update", request);
        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser2);
        await Context.Projects.AddAsync(_project);
        await Context.ProjectTasks.AddAsync(_projectTask);
        await Context.TimeEntries.AddAsync(_timeEntry);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.TimeEntries.RemoveRange(Context.TimeEntries);
        Context.ProjectTasks.RemoveRange(Context.ProjectTasks);
        Context.ProjectUsers.RemoveRange(Context.ProjectUsers);
        Context.Projects.RemoveRange(Context.Projects);
        Context.Users.RemoveRange(Context.Users); 
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}