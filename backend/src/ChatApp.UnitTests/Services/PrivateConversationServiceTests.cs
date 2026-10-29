using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.PrivateConversations.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using ChatApp.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ChatApp.UnitTests.Services;

public class PrivateConversationServiceTests
{
    private readonly Mock<IChatNotifier> _chatNotifierMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();

    [Fact]
    public async Task GetOrCreateAsync_FriendsCreateConversation_ReturnsOtherUser()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (firstUser, secondUser) = await SeedFriendsAsync(dbContext);

        var result = await service.GetOrCreateAsync(firstUser.Id, secondUser.Id);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(secondUser.Id, result.OtherUserId);
        Assert.Equal(secondUser.Username, result.OtherUsername);
        Assert.Single(await dbContext.PrivateConversations.ToListAsync());
    }

    [Fact]
    public async Task GetOrCreateAsync_ExistingConversation_ReturnsSameConversation()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (firstUser, secondUser) = await SeedFriendsAsync(dbContext);

        var firstResult = await service.GetOrCreateAsync(
            firstUser.Id, secondUser.Id);

        var secondResult = await service.GetOrCreateAsync(
            secondUser.Id, firstUser.Id);

        Assert.Equal(firstResult.Id, secondResult.Id);
        Assert.Equal(firstUser.Id, secondResult.OtherUserId);
        Assert.Single(await dbContext.PrivateConversations.ToListAsync());
    }

    [Fact]
    public async Task GetOrCreateAsync_SameUser_ThrowsConflictException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var userId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.GetOrCreateAsync(userId, userId));

        Assert.Equal(
            "You cannot start a conversation with yourself",
            exception.Message);
    }

    [Fact]
    public async Task GetOrCreateAsync_NotFriends_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var firstUser = TestDataFactory.CreateUser(
            Guid.NewGuid(), "first", "first@test.com");
        var secondUser = TestDataFactory.CreateUser(
            Guid.NewGuid(), "second", "second@test.com");

        dbContext.Users.AddRange(firstUser, secondUser);
        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ForbiddenException>(
            () => service.GetOrCreateAsync(firstUser.Id, secondUser.Id));

        Assert.Empty(await dbContext.PrivateConversations.ToListAsync());
    }

    [Fact]
    public async Task SendMessageAsync_SavesMessageAndNotifiesRecipient()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (sender, recipient) = await SeedFriendsAsync(dbContext);
        var conversation = await CreateConversationAsync(
            dbContext, sender.Id, recipient.Id);

        var result = await service.SendMessageAsync(
            sender.Id,
            conversation.Id,
            new CreateMessageRequestDto { Content = "  Hello there  " });

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(conversation.Id, result.ConversationId);
        Assert.Equal(sender.Id, result.UserId);
        Assert.Equal(sender.Username, result.Username);
        Assert.Equal("Hello there", result.Content);

        var savedMessage = await dbContext.PrivateMessages
            .SingleAsync(x => x.Id == result.Id);

        Assert.Equal("Hello there", savedMessage.Content);

        _chatNotifierMock.Verify(
            x => x.PrivateMessageCreatedAsync(
                conversation.Id,
                It.Is<PrivateMessageResponseDto>(message =>
                    message.Id == result.Id &&
                    message.Content == "Hello there")),
            Times.Once);

        _notificationServiceMock.Verify(
            x => x.CreateAsync(
                recipient.Id,
                sender.Id,
                "PrivateMessageCreated",
                $"{sender.Username} sent you a private message",
                null,
                result.Id),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_EmptyContent_ThrowsConflictException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (sender, recipient) = await SeedFriendsAsync(dbContext);
        var conversation = await CreateConversationAsync(
            dbContext, sender.Id, recipient.Id);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.SendMessageAsync(
                sender.Id,
                conversation.Id,
                new CreateMessageRequestDto { Content = "   " }));

        Assert.Empty(await dbContext.PrivateMessages.ToListAsync());
        _chatNotifierMock.Verify(
            x => x.PrivateMessageCreatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<PrivateMessageResponseDto>()),
            Times.Never);
        _notificationServiceMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<Guid?>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateMessageAsync_OwnerUpdatesMessageAndNotifies()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (sender, recipient) = await SeedFriendsAsync(dbContext);
        var conversation = await CreateConversationAsync(
            dbContext, sender.Id, recipient.Id);
        var message = new PrivateMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            UserId = sender.Id,
            Content = "Original",
            CreatedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        dbContext.PrivateMessages.Add(message);
        await dbContext.SaveChangesAsync();

        var result = await service.UpdateMessageAsync(
            sender.Id,
            conversation.Id,
            message.Id,
            new UpdateMessageRequestDto { Content = "  Updated  " });

        Assert.Equal("Updated", result.Content);
        Assert.NotNull(result.UpdatedAt);

        var savedMessage = await dbContext.PrivateMessages
            .SingleAsync(x => x.Id == message.Id);
        Assert.Equal("Updated", savedMessage.Content);
        Assert.NotNull(savedMessage.UpdatedAt);

        _chatNotifierMock.Verify(
            x => x.PrivateMessageUpdatedAsync(
                conversation.Id,
                It.Is<PrivateMessageResponseDto>(updated =>
                    updated.Id == message.Id &&
                    updated.Content == "Updated")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateMessageAsync_NonOwner_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (sender, recipient) = await SeedFriendsAsync(dbContext);
        var conversation = await CreateConversationAsync(
            dbContext, sender.Id, recipient.Id);
        var message = new PrivateMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            UserId = sender.Id,
            Content = "Original"
        };

        dbContext.PrivateMessages.Add(message);
        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ForbiddenException>(
            () => service.UpdateMessageAsync(
                recipient.Id,
                conversation.Id,
                message.Id,
                new UpdateMessageRequestDto { Content = "Changed" }));

        Assert.Equal(
            "Original",
            (await dbContext.PrivateMessages.SingleAsync(
                x => x.Id == message.Id)).Content);

        _chatNotifierMock.Verify(
            x => x.PrivateMessageUpdatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<PrivateMessageResponseDto>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteMessageAsync_OwnerDeletesMessageAndNotifies()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (sender, recipient) = await SeedFriendsAsync(dbContext);
        var conversation = await CreateConversationAsync(
            dbContext, sender.Id, recipient.Id);
        var message = new PrivateMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            UserId = sender.Id,
            Content = "Delete me"
        };

        dbContext.PrivateMessages.Add(message);
        await dbContext.SaveChangesAsync();

        await service.DeleteMessageAsync(
            sender.Id, conversation.Id, message.Id);

        Assert.False(await dbContext.PrivateMessages
            .AnyAsync(x => x.Id == message.Id));

        _chatNotifierMock.Verify(
            x => x.PrivateMessageDeletedAsync(
                conversation.Id,
                It.Is<PrivateMessageDeletedResponseDto>(deleted =>
                    deleted.MessageId == message.Id &&
                    deleted.ConversationId == conversation.Id)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteMessageAsync_NonOwner_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (sender, recipient) = await SeedFriendsAsync(dbContext);
        var conversation = await CreateConversationAsync(
            dbContext, sender.Id, recipient.Id);
        var message = new PrivateMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            UserId = sender.Id,
            Content = "Keep me"
        };

        dbContext.PrivateMessages.Add(message);
        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ForbiddenException>(
            () => service.DeleteMessageAsync(
                recipient.Id, conversation.Id, message.Id));

        Assert.True(await dbContext.PrivateMessages
            .AnyAsync(x => x.Id == message.Id));

        _chatNotifierMock.Verify(
            x => x.PrivateMessageDeletedAsync(
                It.IsAny<Guid>(),
                It.IsAny<PrivateMessageDeletedResponseDto>()),
            Times.Never);
    }

    [Fact]
    public async Task IsParticipantAsync_ReturnsTrueOnlyForConversationParticipants()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (firstUser, secondUser) = await SeedFriendsAsync(dbContext);
        var outsider = TestDataFactory.CreateUser(
            Guid.NewGuid(), "outsider", "outsider@test.com");
        dbContext.Users.Add(outsider);
        await dbContext.SaveChangesAsync();

        var conversation = await CreateConversationAsync(
            dbContext, firstUser.Id, secondUser.Id);

        Assert.True(await service.IsParticipantAsync(
            firstUser.Id, conversation.Id));
        Assert.True(await service.IsParticipantAsync(
            secondUser.Id, conversation.Id));
        Assert.False(await service.IsParticipantAsync(
            outsider.Id, conversation.Id));
        Assert.False(await service.IsParticipantAsync(
            firstUser.Id, Guid.NewGuid()));
    }

    [Fact]
    public async Task GetMessagesAsync_ReturnsPagedMessagesInChronologicalOrder()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = CreateService(dbContext);
        var (sender, recipient) = await SeedFriendsAsync(dbContext);
        var conversation = await CreateConversationAsync(
            dbContext, sender.Id, recipient.Id);
        var start = DateTime.UtcNow.AddMinutes(-3);

        dbContext.PrivateMessages.AddRange(
            new PrivateMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                UserId = sender.Id,
                Content = "First",
                CreatedAt = start
            },
            new PrivateMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                UserId = recipient.Id,
                Content = "Second",
                CreatedAt = start.AddMinutes(1)
            },
            new PrivateMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                UserId = sender.Id,
                Content = "Third",
                CreatedAt = start.AddMinutes(2)
            });

        await dbContext.SaveChangesAsync();

        var result = await service.GetMessagesAsync(
            sender.Id,
            conversation.Id,
            new MessageQueryDto { PageNumber = 1, PageSize = 2 });

        var items = result.Items.ToList();

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Second", items[0].Content);
        Assert.Equal("Third", items[1].Content);
    }

    private PrivateConversationService CreateService(AppDbContext dbContext)
    {
        return new PrivateConversationService(
            dbContext,
            _chatNotifierMock.Object,
            _notificationServiceMock.Object);
    }

    private static async Task<(User First, User Second)> SeedFriendsAsync(
        AppDbContext dbContext)
    {
        var first = TestDataFactory.CreateUser(
            Guid.NewGuid(), "first", "first@test.com");
        var second = TestDataFactory.CreateUser(
            Guid.NewGuid(), "second", "second@test.com");

        dbContext.Users.AddRange(first, second);

        var (user1Id, user2Id) = first.Id.CompareTo(second.Id) < 0
            ? (first.Id, second.Id)
            : (second.Id, first.Id);

        dbContext.Friendships.Add(new Friendship
        {
            Id = Guid.NewGuid(),
            UserId = user1Id,
            FriendUserId = user2Id
        });

        await dbContext.SaveChangesAsync();
        return (first, second);
    }

    private static async Task<PrivateConversation> CreateConversationAsync(
        AppDbContext dbContext,
        Guid firstUserId,
        Guid secondUserId)
    {
        var (user1Id, user2Id) = firstUserId.CompareTo(secondUserId) < 0
            ? (firstUserId, secondUserId)
            : (secondUserId, firstUserId);

        var conversation = new PrivateConversation
        {
            Id = Guid.NewGuid(),
            User1Id = user1Id,
            User2Id = user2Id,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.PrivateConversations.Add(conversation);
        await dbContext.SaveChangesAsync();
        return conversation;
    }
}