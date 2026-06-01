using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Persistence.Configurations;

public class WorkspaceMemberConfiguration
    : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(
        EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.HasKey(x =>
            new
            {
                x.WorkspaceId,
                x.UserId
            });

        builder
            .HasOne(x => x.Workspace)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.WorkspaceId);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.WorkspaceMemberships)
            .HasForeignKey(x => x.UserId);
    }
}