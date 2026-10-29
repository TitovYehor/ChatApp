using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Services;
using ChatApp.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ChatApp.UnitTests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationNotifier> _notifierMock = new();

    [Fact]
    public async Task CreateAsync_PersistsNotificationAndNotifiesRecipient()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = new NotificationService(dbContext, _notifierMock.Object);

        var recipientId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var entityId = Guid.NewGuid();

        var result = await service.CreateAsync(
            recipientId,
            actorId,
            "PrivateMessageCreated",
            "Alice sent you a private message",
            entityId: entityId);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(actorId, result.ActorUserId);
        Assert.Equal("PrivateMessageCreated", result.Type);
        Assert.Equal("Alice sent you a private message", result.Message);
        Assert.Equal(entityId, result.EntityId);
        Assert.False(result.IsRead);

        var saved = await dbContext.Notifications
            .SingleAsync(x => x.Id == result.Id);

        Assert.Equal(recipientId, saved.RecipientUserId);
        Assert.Equal(actorId, saved.ActorUserId);
        Assert.Equal(entityId, saved.EntityId);

        _notifierMock.Verify(x => x.NotificationCreatedAsync(
            recipientId,
            It.Is<ChatApp.Contracts.Notifications.Responses.NotificationResponseDto>(
                n => n.Id == result.Id &&
                     n.Type == "PrivateMessageCreated" &&
                     !n.IsRead)),
            Times.Once);
    }

    [Fact]
    public async Task GetLatestAsync_ReturnsOnlyRecipientNotificationsNewestFirst()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = new NotificationService(dbContext, _notifierMock.Object);

        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var olderId = Guid.NewGuid();
        var newerId = Guid.NewGuid();

        dbContext.Notifications.AddRange(
            new Notification
            {
                Id = olderId,
                RecipientUserId = userId,
                Type = "Older",
                Message = "Older notification",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            },
            new Notification
            {
                Id = newerId,
                RecipientUserId = userId,
                Type = "Newer",
                Message = "Newer notification",
                CreatedAt = DateTime.UtcNow
            },
            new Notification
            {
                RecipientUserId = otherUserId,
                Type = "Other",
                Message = "Belongs to another user",
                CreatedAt = DateTime.UtcNow.AddMinutes(1)
            });

        await dbContext.SaveChangesAsync();

        var result = await service.GetLatestAsync(userId);

        Assert.Equal(2, result.Count);
        Assert.Equal(newerId, result[0].Id);
        Assert.Equal(olderId, result[1].Id);
    }

    [Fact]
    public async Task GetLatestAsync_ClampsLimitToOneThroughOneHundred()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = new NotificationService(dbContext, _notifierMock.Object);
        var userId = Guid.NewGuid();

        dbContext.Notifications.AddRange(
            Enumerable.Range(0, 3).Select(i => new Notification
            {
                RecipientUserId = userId,
                Type = "Test",
                Message = $"Notification {i}",
                CreatedAt = DateTime.UtcNow.AddMinutes(i)
            }));

        await dbContext.SaveChangesAsync();

        var withZeroLimit = await service.GetLatestAsync(userId, 0);
        var withLargeLimit = await service.GetLatestAsync(userId, 1000);

        Assert.Single(withZeroLimit);
        Assert.Equal(3, withLargeLimit.Count);
    }

    [Fact]
    public async Task MarkAsReadAsync_OnlyMarksNotificationForItsRecipient()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();
        var service = new NotificationService(dbContext, _notifierMock.Object);

        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        dbContext.Notifications.Add(new Notification
        {
            Id = notificationId,
            RecipientUserId = ownerId,
            Type = "Test",
            Message = "Unread notification",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        });

        await dbContext.SaveChangesAsync();

        Assert.False(await service.MarkAsReadAsync(notificationId, otherUserId));
        Assert.False((await dbContext.Notifications
            .SingleAsync(x => x.Id == notificationId)).IsRead);

        Assert.True(await service.MarkAsReadAsync(notificationId, ownerId));
        Assert.True((await dbContext.Notifications
            .SingleAsync(x => x.Id == notificationId)).IsRead);

        Assert.True(await service.MarkAsReadAsync(notificationId, ownerId));
        Assert.False(await service.MarkAsReadAsync(Guid.NewGuid(), ownerId));
    }
}