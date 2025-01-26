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
}