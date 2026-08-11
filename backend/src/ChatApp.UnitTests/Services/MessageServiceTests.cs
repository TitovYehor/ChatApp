using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ChatApp.UnitTests.Services;

public class MessageServiceTests
{
    private readonly Mock<IChatNotifier> _chatNotifierMock;

    public MessageServiceTests()
    {
        _chatNotifierMock = new Mock<IChatNotifier>();
    }

    [Fact]
    public async Task CreateAsync_MemberCreatesMessage_ReturnsMessageAndNotifies()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com"
        };

        var workspace = new Workspace
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
                    Role = WorkspaceRole.Member
                }
            ]
        };

        var channel = new Channel
        {
            Id = channelId,
            WorkspaceId = workspaceId,
            Name = "general"
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);

        await dbContext.SaveChangesAsync();

        var request = new CreateMessageRequestDto
        {
            Content = "  Hello world  "
        };

        var result = await service.CreateAsync(
            channelId,
            userId,
            request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(channelId, result.ChannelId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("Hello world", result.Content);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.Null(result.UpdatedAt);

        var savedMessage = await dbContext.Messages
            .FirstAsync(x => x.Id == result.Id);

        Assert.Equal(channelId, savedMessage.ChannelId);
        Assert.Equal(userId, savedMessage.UserId);
        Assert.Equal("Hello world", savedMessage.Content);

        _chatNotifierMock.Verify(
            x => x.MessageCreatedAsync(
                channelId,
                It.Is<MessageResponseDto>(message =>
                    message.Id == result.Id &&
                    message.ChannelId == channelId &&
                    message.UserId == userId &&
                    message.Username == "testuser" &&
                    message.Content == "Hello world")),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ChannelDoesNotExist_ThrowsNotFoundException()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var channelId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = new CreateMessageRequestDto
        {
            Content = "Hello world"
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateAsync(
                channelId,
                userId,
                request));

        Assert.Equal("Channel not found", exception.Message);

        _chatNotifierMock.Verify(
            x => x.MessageCreatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<MessageResponseDto>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UserIsNotWorkspaceMember_ThrowsForbiddenException()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = workspaceId,
            Name = "Test workspace",
            Description = "Test description",
            Members =
            [
                new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = memberId,
                    Role = WorkspaceRole.Member
                }
            ]
        };

        var channel = new Channel
        {
            Id = channelId,
            WorkspaceId = workspaceId,
            Name = "general"
        };

        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);

        await dbContext.SaveChangesAsync();

        var request = new CreateMessageRequestDto
        {
            Content = "Hello world"
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.CreateAsync(
                channelId,
                nonMemberId,
                request));

        Assert.Equal(
            "User is not a member of this workspace",
            exception.Message);

        Assert.Empty(
            await dbContext.Messages.ToListAsync());

        _chatNotifierMock.Verify(
            x => x.MessageCreatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<MessageResponseDto>()),
            Times.Never);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private MessageService CreateMessageService(AppDbContext dbContext)
    {
        return new MessageService(dbContext, _chatNotifierMock.Object);
    }
}