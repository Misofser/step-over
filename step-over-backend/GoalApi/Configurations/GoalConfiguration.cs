using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Models;

namespace GoalApi.Configurations;

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.ToTable("Goals");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title)
            .IsRequired();

        builder.Property(g => g.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(g => g.CreatedAt)
            .IsRequired();
        builder.Property(g => g.UpdatedAt)
            .IsRequired();

        builder.Property(g => g.UserId)
            .IsRequired();

        builder.HasIndex(g => g.UserId);

        builder.HasOne(g => g.User)
           .WithMany(u => u.Goals)
           .HasForeignKey(g => g.UserId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
