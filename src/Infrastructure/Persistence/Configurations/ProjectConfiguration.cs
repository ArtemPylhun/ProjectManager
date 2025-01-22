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
        
        builder.HasOne(a => a.Creator)
            .WithMany(u => u.Projects)
            .HasForeignKey(a => a.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}