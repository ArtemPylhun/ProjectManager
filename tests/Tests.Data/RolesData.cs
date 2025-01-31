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
}