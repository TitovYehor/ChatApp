using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Defines the database mapping and constraints for channels
/// </summary>
public class ChannelConfiguration
    : IEntityTypeConfiguration<Channel>
{
    public void Configure(
        EntityTypeBuilder<Channel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasOne(x => x.Workspace)
            .WithMany(x => x.Channels)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new
            {
                x.WorkspaceId,
                x.Name
            })
            .IsUnique();
    }
}