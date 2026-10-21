using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users 
        => Set<User>();

    public DbSet<Friendship> Friendships 
        => Set<Friendship>();

    public DbSet<Workspace> Workspaces 
        => Set<Workspace>();

    public DbSet<WorkspaceMember> WorkspaceMembers
        => Set<WorkspaceMember>();

    public DbSet<Channel> Channels
        => Set<Channel>();

    public DbSet<Message> Messages
        => Set<Message>();

    public DbSet<RefreshToken> RefreshTokens
        => Set<RefreshToken>();

    public DbSet<Notification> Notifications
        => Set<Notification>();

    public DbSet<PrivateConversation> PrivateConversations
    => Set<PrivateConversation>();

    public DbSet<PrivateMessage> PrivateMessages
        => Set<PrivateMessage>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}