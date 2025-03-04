using Domain.Models.Roles;

namespace Tests.Data;

public static class RolesData
{
    public static Role UserRole =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "User",
            NormalizedName = "User".ToUpperInvariant(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

    public static Role AdminRole2 =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin2",
            NormalizedName = "Admin2".ToUpperInvariant(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
    
    public static Role AdminRole =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            NormalizedName = "Admin".ToUpperInvariant(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

    public static Role Creator1Role =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "Creator1",
            NormalizedName = "Creator1".ToUpperInvariant(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

    public static Role ProjectRole =>
        new Role
        {
            Id = Guid.NewGuid(),
            Name = "TeamLead",
            RoleGroup = RoleGroups.Projects,
            NormalizedName = "TeamLead".ToUpperInvariant(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
}