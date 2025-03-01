using Domain.Models.Projects;
using Domain.Models.ProjectUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProjectUserConfiguration: IEntityTypeConfiguration<ProjectUser>
{
    public void Configure(EntityTypeBuilder<ProjectUser> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ProjectUserId(x));
        
        builder.Property(x => x.ProjectId).HasConversion(x => x.Value, x => new ProjectId(x));
        
        builder
            .HasIndex(pu => new { pu.UserId, pu.ProjectId, pu.RoleId })
            .IsUnique();
        
        builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectUsers)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.ProjectUsers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Role)
            .WithMany(x => x.ProjectUsers)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}