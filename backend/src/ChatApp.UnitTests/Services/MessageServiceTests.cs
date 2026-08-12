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

        var (user, workspace, channel) =
            CreateMessageContext(
                userId,
                workspaceId,
                channelId);

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

        var (workspace, channel) =
            CreateMessageContextWithoutUser(
                memberId,
                workspaceId,
                channelId);

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

    [Fact]
    public async Task GetByIdAsync_MemberRetrievesMessage_ReturnsMessage()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                userId,
                workspaceId,
                channelId);

        var message = new Message
        {
            Id = messageId,
            ChannelId = channelId,
            UserId = userId,
            Content = "Hello world",
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        var result = await service.GetByIdAsync(
            messageId,
            userId);

        Assert.Equal(messageId, result.Id);
        Assert.Equal(channelId, result.ChannelId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("Hello world", result.Content);
        Assert.Equal(message.CreatedAt, result.CreatedAt);
        Assert.Null(result.UpdatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_MessageDoesNotExist_ThrowsNotFoundException()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var messageId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByIdAsync(
                messageId,
                userId));

        Assert.Equal("Message not found", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_UserIsNotWorkspaceMember_ThrowsNotFoundException()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var memberId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                memberId,
                workspaceId,
                channelId);

        var message = new Message
        {
            Id = messageId,
            ChannelId = channelId,
            UserId = memberId,
            Content = "Secret message"
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByIdAsync(
                messageId,
                nonMemberId));

        Assert.Equal("Message not found", exception.Message);
    }

    [Fact]
    public async Task GetByChannelIdAsync_MemberGetsMessages_ReturnsPagedResult()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                userId,
                workspaceId,
                channelId);

        var firstMessage = new Message
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            UserId = userId,
            Content = "First message",
            CreatedAt = DateTime.UtcNow.AddMinutes(-3)
        };

        var secondMessage = new Message
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            UserId = userId,
            Content = "Second message",
            CreatedAt = DateTime.UtcNow.AddMinutes(-2)
        };

        var thirdMessage = new Message
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            UserId = userId,
            Content = "Third message",
            CreatedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.AddRange(
            firstMessage,
            secondMessage,
            thirdMessage);

        await dbContext.SaveChangesAsync();

        var query = new MessageQueryDto
        {
            PageNumber = 2,
            PageSize = 2
        };

        var result = await service.GetByChannelIdAsync(
            channelId,
            userId,
            query);

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalCount);

        Assert.Single(result.Items);

        var returnedMessage = result.Items.Single();

        Assert.Equal(thirdMessage.Id, returnedMessage.Id);
        Assert.Equal("Third message", returnedMessage.Content);
        Assert.Equal("testuser", returnedMessage.Username);
    }

    [Fact]
    public async Task GetByChannelIdAsync_WithSearch_ReturnsMatchingMessages()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                userId,
                workspaceId,
                channelId);

        var matchingMessage = new Message
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            UserId = userId,
            Content = "Hello from the backend",
            CreatedAt = DateTime.UtcNow.AddMinutes(-2)
        };

        var nonMatchingMessage = new Message
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            UserId = userId,
            Content = "Something completely different",
            CreatedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.AddRange(
            matchingMessage,
            nonMatchingMessage);

        await dbContext.SaveChangesAsync();

        var query = new MessageQueryDto
        {
            PageNumber = 1,
            PageSize = 50,
            Search = "backend"
        };

        var result = await service.GetByChannelIdAsync(
            channelId,
            userId,
            query);

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);

        var returnedMessage = result.Items.Single();

        Assert.Equal(
            matchingMessage.Id,
            returnedMessage.Id);

        Assert.Equal(
            "Hello from the backend",
            returnedMessage.Content);
    }

    [Fact]
    public async Task GetByChannelIdAsync_UserIsNotWorkspaceMember_ThrowsNotFoundException()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var memberId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                memberId,
                workspaceId,
                channelId);

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);

        await dbContext.SaveChangesAsync();

        var query = new MessageQueryDto();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByChannelIdAsync(
                channelId,
                nonMemberId,
                query));

        Assert.Equal("Channel not found", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_MessageOwnerUpdatesMessage_ReturnsUpdatedMessageAndNotifies()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                userId,
                workspaceId,
                channelId);

        var originalCreatedAt = DateTime.UtcNow.AddMinutes(-5);

        var message = new Message
        {
            Id = messageId,
            ChannelId = channelId,
            UserId = userId,
            Content = "Old content",
            CreatedAt = originalCreatedAt
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        var request = new UpdateMessageRequestDto
        {
            Content = "  New content  "
        };

        var result = await service.UpdateAsync(
            messageId,
            userId,
            request);

        Assert.Equal(messageId, result.Id);
        Assert.Equal(channelId, result.ChannelId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("New content", result.Content);
        Assert.Equal("testuser", result.Username);
        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.NotNull(result.UpdatedAt);

        var savedMessage = await dbContext.Messages
            .FirstAsync(x => x.Id == messageId);

        Assert.Equal("New content", savedMessage.Content);
        Assert.NotNull(savedMessage.UpdatedAt);

        _chatNotifierMock.Verify(
            x => x.MessageUpdatedAsync(
                channelId,
                It.Is<MessageResponseDto>(response =>
                    response.Id == messageId &&
                    response.ChannelId == channelId &&
                    response.UserId == userId &&
                    response.Username == "testuser" &&
                    response.Content == "New content" &&
                    response.UpdatedAt != null)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UserIsNotMessageOwner_ThrowsNotFoundException()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var (owner, workspace, channel) =
            CreateMessageContext(
                ownerId,
                workspaceId,
                channelId);

        var message = new Message
        {
            Id = messageId,
            ChannelId = channelId,
            UserId = ownerId,
            Content = "Original content"
        };

        dbContext.Users.Add(owner);

        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        var request = new UpdateMessageRequestDto
        {
            Content = "Changed content"
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateAsync(
                messageId,
                otherUserId,
                request));

        Assert.Equal("Message not found", exception.Message);

        _chatNotifierMock.Verify(
            x => x.MessageUpdatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<MessageResponseDto>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ContentIsUnchanged_ReturnsMessageWithoutUpdatingOrNotifying()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                userId,
                workspaceId,
                channelId);

        var createdAt = DateTime.UtcNow.AddMinutes(-5);
        var updatedAt = DateTime.UtcNow.AddMinutes(-2);

        var message = new Message
        {
            Id = messageId,
            ChannelId = channelId,
            UserId = userId,
            Content = "Same content",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        var request = new UpdateMessageRequestDto
        {
            Content = "  Same content  "
        };

        var result = await service.UpdateAsync(
            messageId,
            userId,
            request);

        Assert.Equal(messageId, result.Id);
        Assert.Equal("Same content", result.Content);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(updatedAt, result.UpdatedAt);

        var savedMessage = await dbContext.Messages
            .FirstAsync(x => x.Id == messageId);

        Assert.Equal("Same content", savedMessage.Content);
        Assert.Equal(updatedAt, savedMessage.UpdatedAt);

        _chatNotifierMock.Verify(
            x => x.MessageUpdatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<MessageResponseDto>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_MessageOwnerDeletesMessage_RemovesMessageAndNotifies()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var (user, workspace, channel) =
            CreateMessageContext(
                userId,
                workspaceId,
                channelId);

        var message = new Message
        {
            Id = messageId,
            ChannelId = channelId,
            UserId = userId,
            Content = "Message to delete"
        };

        dbContext.Users.Add(user);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        await service.DeleteAsync(
            messageId,
            userId);

        var deletedMessage = await dbContext.Messages
            .FirstOrDefaultAsync(x => x.Id == messageId);

        Assert.Null(deletedMessage);

        _chatNotifierMock.Verify(
            x => x.MessageDeletedAsync(
                channelId,
                It.Is<MessageDeletedResponseDto>(response =>
                    response.MessageId == messageId &&
                    response.ChannelId == channelId)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_UserIsNotMessageOwner_ThrowsNotFoundException()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateMessageService(dbContext);

        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var (owner, workspace, channel) =
            CreateMessageContext(
                ownerId,
                workspaceId,
                channelId);

        var message = new Message
        {
            Id = messageId,
            ChannelId = channelId,
            UserId = ownerId,
            Content = "Original content"
        };

        dbContext.Users.Add(owner);
        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);
        dbContext.Messages.Add(message);

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteAsync(
                messageId,
                otherUserId));

        Assert.Equal("Message not found", exception.Message);

        var savedMessage = await dbContext.Messages
            .FirstOrDefaultAsync(x => x.Id == messageId);

        Assert.NotNull(savedMessage);

        _chatNotifierMock.Verify(
            x => x.MessageDeletedAsync(
                It.IsAny<Guid>(),
                It.IsAny<MessageDeletedResponseDto>()),
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

    private static (User User, Workspace Workspace, Channel Channel) CreateMessageContext(
        Guid userId,
        Guid workspaceId,
        Guid channelId)
    {
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

        return (user, workspace, channel);
    }

    private static (Workspace Workspace, Channel Channel) CreateMessageContextWithoutUser(
        Guid userId,
        Guid workspaceId,
        Guid channelId)
    {
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

        return (workspace, channel);
    }
}