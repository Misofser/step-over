using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoalApi.Models;
using GoalApi.Enums;

namespace GoalApi.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Type)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => Enum.Parse<WorkspaceType>(v, true)
            )
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();
        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.HasMany(w => w.Members)
            .WithOne(m => m.Workspace)
            .HasForeignKey(m => m.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
