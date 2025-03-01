using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);

        builder.Property(x => x.UserName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(255)");
        
        builder.HasMany(x => x.TimeEntries)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId);
        
        builder.HasMany(x => x.ProjectUsers)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId);
    }
}