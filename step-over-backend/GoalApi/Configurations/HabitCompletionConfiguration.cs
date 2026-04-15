using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Models;

namespace GoalApi.Configurations;

public class HabitCompletionConfiguration : IEntityTypeConfiguration<HabitCompletion>
{
    public void Configure(EntityTypeBuilder<HabitCompletion> builder)
    {
        builder.ToTable("HabitCompletions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();
        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.HasOne(c => c.Habit)
               .WithMany(h => h.Completions)
               .HasForeignKey(c => c.HabitId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.HabitId, c.Date })
               .IsUnique();
    }
}
