using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Persistence.Configurations;

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
            .HasForeignKey(x => x.WorkspaceId);

        builder
            .HasIndex(x => new
            {
                x.WorkspaceId,
                x.Name
            })
            .IsUnique();
    }
}