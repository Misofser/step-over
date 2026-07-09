using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Models;
using GoalApi.Enums;

namespace GoalApi.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("WorkspaceMembers");

        builder.HasKey(wm => wm.Id);

        builder.Property(wm => wm.Role)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<WorkspaceRole>(v, true)
            )
            .IsRequired();

        builder.Property(wm => wm.UserId)
            .IsRequired();
        builder.HasIndex(wm => wm.UserId);

        builder.Property(wm => wm.WorkspaceId)
            .IsRequired();
        builder.HasIndex(wm => wm.WorkspaceId);

        builder.Property(wm => wm.CreatedAt)
            .IsRequired();
        builder.Property(wm => wm.UpdatedAt)
            .IsRequired();

        builder.HasOne(wm => wm.Workspace)
            .WithMany(w => w.Members)
            .HasForeignKey(wm => wm.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wm => wm.User)
            .WithMany(u => u.WorkspaceMembers)
            .HasForeignKey(wm => wm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(wm => new { wm.WorkspaceId, wm.UserId })
            .IsUnique();
    }
}
