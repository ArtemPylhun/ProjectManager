using Domain.Models.Roles;

namespace Tests.Data;

public static class RolesData
{
    public static Role UserRole =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "User",
            RoleGroup = RoleGroups.General,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
    
    public static Role UserRole2 =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "User2",
            RoleGroup = RoleGroups.General,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

    public static Role AdminRole2 =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin2",
            RoleGroup = RoleGroups.General,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
    
    public static Role AdminRole3 =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin3",
            RoleGroup = RoleGroups.General,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
    
    public static Role AdminRole =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            RoleGroup = RoleGroups.General,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

    public static Role Creator1Role =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Creator1",
            RoleGroup = RoleGroups.Projects,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

    public static Role ProjectRole =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "TeamLead",
            RoleGroup = RoleGroups.Projects,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
}