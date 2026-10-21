using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Persistence.Configurations;

public class PrivateConversationConfiguration
    : IEntityTypeConfiguration<PrivateConversation>
{
    public void Configure(EntityTypeBuilder<PrivateConversation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User1)
            .WithMany()
            .HasForeignKey(x => x.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User2)
            .WithMany()
            .HasForeignKey(x => x.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.User1Id, x.User2Id })
            .IsUnique();

        builder.HasIndex(x => x.User2Id);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_PrivateConversations_UserOrder",
                "\"User1Id\" < \"User2Id\"");
        });
    }
}