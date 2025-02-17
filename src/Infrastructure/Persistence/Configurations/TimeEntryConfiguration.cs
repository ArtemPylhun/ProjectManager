using Domain.Models.Projects;
using Domain.Models.ProjectTasks;
using Domain.Models.TimeEntries;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TimeEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new TimeEntryId(x));

        builder.Property(x => x.ProjectId).HasConversion(x => x.Value, x => new ProjectId(x));
        builder.Property(x => x.ProjectTaskId).HasConversion(x => x.Value, x => new ProjectTaskId(x));

        builder.HasOne(x => x.User)
            .WithMany(x => x.TimeEntries)
            .HasForeignKey(x => x.UserId);
        
        builder.HasOne(x => x.Project)
            .WithMany(x => x.TimeEntries)
            .HasForeignKey(x => x.ProjectId);
        
        builder.HasOne(x => x.ProjectTask)
            .WithMany(x => x.TimeEntries)
            .HasForeignKey(x => x.ProjectTaskId);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasColumnType("varchar(255)");
        
        builder.Property(x => x.StartDate)
            .IsRequired()
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.EndDate)
            .IsRequired()
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.Minutes).IsRequired();
    }
}