using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Roles;
using Domain.Models.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _newUser;
    private readonly User _mainUser;
    private readonly User _mainUser2;
    private readonly User _userForDeletion;
    private readonly Role _adminRole;
    private readonly Role _userRole;
    private string _password = "Admin!23";

    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _newUser = UsersData.NewUser;
        _mainUser = UsersData.MainUser;
        _mainUser2 = UsersData.MainUser2;
        _userForDeletion = UsersData.UserForDeletion;
        _adminRole = RolesData.AdminRole;
        _userRole = RolesData.UserRole;
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        var email = "userTestCreate@gmail.com";
        var userName = "UserName12345";
        var request = new UserCreateDto(
            Email: email,
            UserName: userName,
            Password: _password);

        // Act
        var response = await Client.PostAsJsonAsync("users/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var responseUser = await response.ToResponseModel<UserDto>();
        var userId = responseUser.Id;

        var dbUser = await Context.Users.FirstAsync(x => x.Id == userId);
        dbUser.UserName.Should().Be(userName);
        dbUser.Email.Should().Be(email);
        dbUser.PasswordHash.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldNotCreateUserBecauseNameDuplicated()
    {
        // Arrange
        var userName = _mainUser2.UserName;
        var email = "userTestCreateName@gmail.com";
        var request = new UserCreateDto(
            Email: email,
            UserName: userName,
            Password: _password);

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
        var request = new UserCreateDto(
            Email: email,
            UserName: userName,
            Password: _password);

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
        var email = "testEmail@gmail.com";
        var password = _password;
        var request = new UserUpdateDto(
            Id: _mainUser.Id,
            Email: email,
            UserName: userName
            );

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
        var password = _password;
        var request = new UserUpdateDto(
            Id: _newUser.Id,
            Email: email,
            UserName: userName
            );

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
        var userName = _mainUser2.UserName;
        var password = _password;
        var request = new UserUpdateDto(
            Id: _mainUser.Id,
            Email: _mainUser2.Email,
            UserName: userName
            );

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
        var email = _mainUser2.Email;
        var password = _password;
        var request = new UserUpdateDto(
            Id: _mainUser.Id,
            Email: email,
            UserName: _mainUser.UserName
            );

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
        var password = _password;
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
        var password = _password;
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
    
    [Fact]
    public async Task ShouldUpdateUserRolesSuccessfully()
    {
        // Arrange
        var testUser = _mainUser;
        var newRoles = new List<string> { "User", "Admin" };

        // Act
        var response = await Client.PutAsJsonAsync($"users/{testUser.Id}/update-roles", newRoles);
    
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Force fresh DB read
        await Context.Entry(testUser).ReloadAsync();
    
        var assignedRolesIds = await Context.UserRoles
            .Where(x => x.UserId == testUser.Id)
            .Select(x => x.RoleId)
            .ToListAsync();

        var assignedRoles = await Context.Roles
            .Where(x => assignedRolesIds.Contains(x.Id))
            .Select(x => x.Name)
            .ToListAsync();

        assignedRoles.Should().BeEquivalentTo(newRoles);
    }
    
    public async Task InitializeAsync()
    {
        await Context.Users.AddAsync(_mainUser);
        await Context.Users.AddAsync(_mainUser2);
        await Context.Users.AddAsync(_userForDeletion);
        await Context.Roles.AddAsync(_userRole);
        await Context.Roles.AddAsync(_adminRole);
        await Context.UserRoles.AddAsync(new IdentityUserRole<Guid> { UserId = _mainUser.Id, RoleId = _adminRole.Id });
        await Context.UserRoles.AddAsync(new IdentityUserRole<Guid> { UserId = _mainUser.Id, RoleId = _userRole.Id });
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.UserRoles.RemoveRange(Context.UserRoles);
        Context.Projects.RemoveRange(Context.Projects);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}