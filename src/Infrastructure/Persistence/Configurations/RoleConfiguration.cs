using Domain.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        
        builder.HasMany<IdentityRoleClaim<Guid>>()
            .WithOne()
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();

        builder.HasMany(x => x.ProjectUsers)
            .WithOne(pu => pu.Role)
            .HasForeignKey(pu => pu.RoleId);
    }
}