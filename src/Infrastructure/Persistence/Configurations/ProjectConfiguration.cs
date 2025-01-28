using Domain.Models.Projects;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ProjectId(x));
        
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.HasOne(p => p.Creator) 
            .WithMany(u => u.CreatedProjects) 
            .HasForeignKey(p => p.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Client) 
            .WithMany(u => u.ClientProjects) 
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        builder.HasMany(x => x.TimeEntries)
            .WithOne(u => u.Project)
            .HasForeignKey(u => u.ProjectId);
        
        builder.HasMany(x => x.ProjectTasks)
            .WithOne(u => u.Project)
            .HasForeignKey(u => u.ProjectId);
        
        builder.HasMany(x => x.ProjectUsers)
            .WithOne(u => u.Project)
            .HasForeignKey(u => u.ProjectId);
    }
}