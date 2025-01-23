using Domain.Models.EmailNotifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EmailNotificationConfiguration: IEntityTypeConfiguration<EmailNotification>
{
    public void Configure(EntityTypeBuilder<EmailNotification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new EmailNotificationId(x));

        builder.Property(x => x.NotificationType)
            .HasConversion(
                x => (int)x,
                x => (NotificationType)x);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Subject).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Body).IsRequired().HasColumnType("varchar(255)");
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.EmailNotifications)
            .HasForeignKey(x => x.UserId);
    }
}