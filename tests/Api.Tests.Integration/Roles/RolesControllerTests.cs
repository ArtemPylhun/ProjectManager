using System.Net;
using System.Net.Http.Json;
using API.DTOs;
using Domain.Models.Roles;
using FluentAssertions;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Roles;

public class RolesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _userRole;
    private readonly Role _adminRole;
    
    public RolesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _userRole = RolesData.UserRole;
        _adminRole = RolesData.AdminRole;
    }

    
    [Fact]
    public async Task ShouldCreateRole()
    {
        // Arrange
        var name = "TestRole";
        var request = new RoleDto
        (
            Id: null,
            Name: name
        );

        // Act
        var response = await Client.PostAsJsonAsync("roles/create", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var createdRole = await response.ToResponseModel<Role>();
        createdRole.Id.Should().NotBeEmpty();
        createdRole.Name.Should().Be(request.Name);
    }
    
    [Fact]
    public async Task ShouldNotCreateRoleBecauseNameDuplicated()
    {
        // Arrange
        var role = new RoleDto
        (
            Id: null,
            Name: _userRole.Name
        );

        // Act
        var response = await Client.PostAsJsonAsync("roles/create", role);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    [Fact]
    public async Task ShouldDeleteRole()
    {
        // Arrange
        var role = _userRole;

        // Act
        var response = await Client.DeleteAsync($"roles/delete/{role.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var dbRole = await Context.Roles.FindAsync(role.Id);
        dbRole.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteRoleBecauseNotFound()
    {
        // Arrange
        var roleNotFound = "RoleNotFound";
        var role = new RoleDto
        (
            Id: null,
            Name: roleNotFound
        );

        // Act
        var response = await Client.DeleteAsync($"roles/delete/{role.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
    }
    [Fact]
    public async Task ShouldUpdateRole()
    {
        // Arrange
        var role = _userRole;
        var newName = "NewRoleName";

        // Act
        var request = new RoleDto
        (
            Id: role.Id,
            Name: newName
        );

        var response = await Client.PutAsJsonAsync("roles/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var dbRole = await Context.Roles.FindAsync(role.Id);
        dbRole!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task ShouldNotUpdateRoleBecauseIdNotFound()
    {
        // Arrange
        var newName = "NewRoleName";

        // Act
        var request = new RoleDto
        (
            Id: Guid.NewGuid(),
            Name: newName
        );

        var response = await Client.PutAsJsonAsync("roles/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ShouldNotUpdateRoleBecauseSuchRoleAlreadyExists()
    {
        // Arrange

        // Act
        var request = new RoleDto
        (
            Id: _userRole.Id,
            Name: _adminRole.Name
        );

        var response = await Client.PutAsJsonAsync("roles/update", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    public async Task InitializeAsync()
    {
        await Context.Roles.AddAsync(_userRole);
        await Context.Roles.AddAsync(_adminRole);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.UserRoles.RemoveRange(Context.UserRoles);
        Context.Roles.RemoveRange(Context.Roles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}