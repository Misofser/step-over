using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Models;
using GoalApi.Enums;

namespace GoalApi.Configurations;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("Habit");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Title)
            .IsRequired();

        builder.Property(h => h.Frequency)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<HabitFrequency>(v, true)
            )
            .IsRequired();

        builder.Property(h => h.CreatedAt)
            .IsRequired();
        builder.Property(h => h.UpdatedAt)
            .IsRequired();

        builder.HasOne(t => t.Goal)
               .WithMany(h => h.Habits)
               .HasForeignKey(t => t.GoalId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Completions)
               .WithOne(c => c.Habit)
               .HasForeignKey(c => c.HabitId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
