using Domain.Models.ProjectTasks;
using Domain.Models.UsersTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserTaskConfiguration: IEntityTypeConfiguration<UserTask>
{
    public void Configure(EntityTypeBuilder<UserTask> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserTaskId(x));

        builder.Property(x => x.ProjectTaskId).HasConversion(x => x.Value, x => new ProjectTaskId(x));
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserTasks)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.ProjectTask)
            .WithMany(x => x.UsersTask)
            .HasForeignKey(x => x.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}