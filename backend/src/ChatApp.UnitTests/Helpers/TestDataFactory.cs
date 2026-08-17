using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.UnitTests.Helpers;

public static class TestDataFactory
{
    public static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static Workspace CreateWorkspace(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role = WorkspaceRole.Owner)
    {
        return new Workspace
        {
            Id = workspaceId,
            Name = "Test workspace",
            Description = "Test description",
            Members =
            [
                new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = userId,
                    Role = role
                }
            ]
        };
    }

    public static Channel CreateChannel(
        Guid channelId,
        Guid workspaceId,
        string name = "general",
        ChannelType type = ChannelType.Text)
    {
        return new Channel
        {
            Id = channelId,
            WorkspaceId = workspaceId,
            Name = name,
            Type = type
        };
    }

    public static User CreateUser(
        Guid userId,
        string username = "testuser",
        string email = "test@example.com")
    {
        return new User
        {
            Id = userId,
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test-password-hash")
        };
    }

    public static WorkspaceMember CreateWorkspaceMember(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role)
    {
        return new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = role
        };
    }
}