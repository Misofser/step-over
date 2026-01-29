using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Models;

namespace GoalApi.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
               .IsRequired();

        builder.HasIndex(u => u.Username)
               .IsUnique();

        builder.Property(u => u.PasswordHash)
               .IsRequired();

        builder.Property(u => u.Role)
               .IsRequired()
               .HasDefaultValue("User"); 

        builder.Property(u => u.CreatedAt)
               .IsRequired();
        builder.Property(u => u.UpdatedAt)
               .IsRequired();
    }
}
