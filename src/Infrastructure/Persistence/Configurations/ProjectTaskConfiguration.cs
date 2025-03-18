using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProjectTaskConfiguration: IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ProjectTaskId(x));
        
        builder.Property(x => x.ProjectId).HasConversion(x => x.Value, x => new ProjectId(x));
        
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectTasks)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(p => p.Creator) 
            .WithMany(u => u.CreatedProjectTasks) 
            .HasForeignKey(p => p.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Status)
        .HasConversion(
            v => v.ToString(),
            v => (ProjectTask.ProjectTaskStatuses)Enum.Parse(typeof(ProjectTask.ProjectTaskStatuses), v)
        );
        
        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.EstimatedTime).IsRequired();
    }
}