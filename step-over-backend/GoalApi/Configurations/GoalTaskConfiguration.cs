using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Models;

namespace GoalApi.Configurations;

public class GoalTaskConfiguration : IEntityTypeConfiguration<GoalTask>
{
    public void Configure(EntityTypeBuilder<GoalTask> builder)
    {
        builder.ToTable("GoalTasks");

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

        builder.HasOne(t => t.Goal)
               .WithMany(g => g.GoalTasks)
               .HasForeignKey(t => t.GoalId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
