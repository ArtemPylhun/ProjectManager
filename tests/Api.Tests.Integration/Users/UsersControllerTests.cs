using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Roles;
using Domain.Models.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _newUser;
    private readonly User _mainUser;
    private readonly User _userForDeletion;
    private readonly Role _userRole;

    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _newUser = UsersData.NewUser;
        _mainUser = UsersData.MainUser;
        _userRole = RolesData.UserRole;
        _userForDeletion = UsersData.UserForDeletion;
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        var email = "userTestCreate@gmail.com";
        var userName = "UserName12345";
        var password = "Admin$23";
        var request = new UserDto(
            Id: null,
            Email: email,
            UserName: userName,
            Password: password);

        // Act
        var response = await Client.PostAsJsonAsync("users/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseUser = await response.ToResponseModel<UserDto>();
        var userId = responseUser.Id!;

        var dbUser = await Context.Users.FirstAsync(x => x.Id == userId);
        dbUser.UserName.Should().Be(userName);
        dbUser.Email.Should().Be(email);
        dbUser.PasswordHash.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldNotCreateUserBecauseNameDuplicated()
    {
        // Arrange
        var userName = "UserName";
        var email = "userTestCreateName@gmail.com";
        var password = "Password!23";
        var request = new UserDto(
            Id: null,
            Email: email,
            UserName: userName,
            Password: password);

        // Act
        var response = await Client.PostAsJsonAsync("users/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotCreateUserBecauseEmailDuplicated()
    {
        // Arrange
        var userName = "UserName123";
        var email = "userName@gmail.com";
        var password = "Password!23";
        var request = new UserDto(
            Id: null,
            Email: email,
            UserName: userName,
            Password: password);

        // Act
        var response = await Client.PostAsJsonAsync("users/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldDeleteUser()
    {
        // Arrange
        var userId = _userForDeletion.Id;

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var dbUser = await Context.Roles.FindAsync(userId);
        dbUser.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteUserBecauseNotFound()
    {
        // Arrange
        var userId = _newUser.Id;

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    
    [Fact]
    public async Task ShouldUpdateUser()
    {
        // Arrange
        var userName = "CreateUserName";
        var password = "Password!23";
        var email = "testEmail@gmail.com";
        var request = new UserDto(
            Id: _mainUser.Id,
            Email: email,
            UserName: userName,
            Password: password);

        // Act
        var response = await Client.PutAsJsonAsync("users/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseUser = await response.ToResponseModel<UserDto>();
        var userId = responseUser.Id;

        var dbUser = await Context.Users.FirstAsync(x => x.Id == userId);
        dbUser.Id.Should().Be(_mainUser.Id);
        dbUser.UserName.Should().Be(userName);
        dbUser.Email.Should().Be(email);
    }

    [Fact]
    public async Task ShouldNotUpdateUserBecauseUserNotFound()
    {
        var userName = "Update User Name";
        var email = "updateUserName@gmail.com";
        var password = "password";
        var request = new UserDto(
            Id: _newUser.Id,
            Email: email,
            UserName: userName,
            Password: password);

        // Act
        var response = await Client.PutAsJsonAsync("users/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    
    
    
    [Fact]
    public async Task ShouldNotUpdateUserBecauseUserNameDuplicated()
    {
        // Arrange
        var userName = "UserName";
        var password = "password";
        var request = new UserDto(
            Id: _mainUser.Id,
            Email: _mainUser.Email,
            UserName: userName,
            Password: password);

        // Act
        var response = await Client.PutAsJsonAsync("users/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotUpdateUserBecauseEmailDuplicated()
    {
        // Arrange
        var email = "testEmail@gmail.com";
        var password = "password";
        var request = new UserDto(
            Id: _mainUser.Id,
            Email: email,
            UserName: _mainUser.UserName,
            Password: password);

        // Act
        var response = await Client.PutAsJsonAsync("users/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    

    [Fact]
    public async Task ShouldLoginUser()
    {
        // Arrange
        var password = "Admin!23";
        var request = new UserLoginDto(
            EmailOrUsername: _mainUser.Email,
            Password: password
        );

        // Act
        var response = await Client.PostAsJsonAsync("users/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var result = await response.ToResponseModel();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldNotLoginUserBecauseNotFoundByEmail()
    {
        // Arrange
        var email = "notFoundEmail@gmail.com";
        var password = "Admin!23";
        var request = new UserLoginDto(
            EmailOrUsername: email,
            Password: password
        );

        // Act
        var response = await Client.PostAsJsonAsync("users/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotLoginUserBecausePasswordNotMatch()
    {
        // Arrange
        var email = _mainUser.Email;
        var userName = "notFoundUsername";
        var password = "wrongPassword!233";
        var request = new UserLoginDto(
            EmailOrUsername: email,
            Password: password
        );

        // Act
        var response = await Client.PostAsJsonAsync("users/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
/*
    [Fact]
    public async Task ShouldChangeUserRole()
    {
        // Arrange
        var existingRole = _adminRole;
        var request = new UpdateUserRoleDto(
            UserId: _mainUser.Id.Value,
            RoleId: existingRole.Id.Value);

        // Act
        var response = await Client.PutAsJsonAsync("users/setRole", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var dbUser = await Context.Users.FirstAsync(x => x.Id == _mainUser.Id);
        dbUser.RoleId.Value.Should().Be(existingRole.Id.Value);
    }

    [Fact]
    public async Task ShouldNotChangeUserRoleBecauseUserNotFound()
    {
        // Arrange
        var existingRole = _adminRole;
        var request = new UpdateUserRoleDto(UserId: Guid.NewGuid(),
            RoleId: existingRole.Id.Value);

        // Act
        var response = await Client.PutAsJsonAsync("users/setRole", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotChangeUserRoleBecauseRoleNotFound()
    {
        // Arrange
        var request = new UpdateUserRoleDto(
            UserId: _mainUser.Id.Value,
            RoleId: Guid.NewGuid());

        // Act
        var response = await Client.PutAsJsonAsync("users/setRole", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }*/

    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await Context.Users.AddAsync(_userForDeletion);
        await Context.Roles.AddAsync(_userRole);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        Context.Projects.RemoveRange(Context.Projects);
        await SaveChangesAsync();
    }
}