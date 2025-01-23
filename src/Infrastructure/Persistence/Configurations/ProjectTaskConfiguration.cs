using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
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
        
        builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectTasks)
            .HasForeignKey(x => x.ProjectId);
        
        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.EstimatedTime).IsRequired();
    }
}