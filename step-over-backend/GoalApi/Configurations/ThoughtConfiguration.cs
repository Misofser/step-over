using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Enums;
using GoalApi.Models;

namespace GoalApi.Configurations;

public class ThoughtConfiguration : IEntityTypeConfiguration<Thought>
{
    public void Configure(EntityTypeBuilder<Thought> builder)
    {
        builder.ToTable("Thoughts");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Content)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<ThoughtStatus>(v, true)
            )
            .IsRequired()
            .HasDefaultValue(ThoughtStatus.Inbox);

        builder.Property(t => t.CreatedAt)
            .IsRequired();
        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.HasIndex(t => t.UserId);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Thoughts)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.WorkspaceId)
            .IsRequired();

        builder.HasIndex(t => t.WorkspaceId);

        builder.HasOne(t => t.Workspace)
            .WithMany(w => w.Thoughts)
            .HasForeignKey(t => t.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
