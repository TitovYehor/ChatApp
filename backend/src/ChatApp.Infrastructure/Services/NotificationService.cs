using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Notifications.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public sealed class NotificationService : INotificationService
{
    private readonly AppDbContext _dbContext;
    private readonly INotificationNotifier _notificationNotifier;

    public NotificationService(
        AppDbContext dbContext,
        INotificationNotifier notificationNotifier)
    {
        _dbContext = dbContext;
        _notificationNotifier = notificationNotifier;
    }

    public async Task<NotificationResponseDto> CreateAsync(
        Guid recipientUserId,
        Guid? actorUserId,
        string type,
        string message,
        Guid? workspaceId = null,
        Guid? entityId = null)
    {
        var notification = new Notification
        {
            RecipientUserId = recipientUserId,
            ActorUserId = actorUserId,
            Type = type,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            WorkspaceId = workspaceId,
            EntityId = entityId
        };

        _dbContext.Notifications.Add(notification);

        await _dbContext.SaveChangesAsync();

        var response = ToResponse(notification);

        await _notificationNotifier.NotificationCreatedAsync(
            recipientUserId,
            response);

        return response;
    }

    public async Task<IReadOnlyList<NotificationResponseDto>> GetLatestAsync(
        Guid userId,
        int limit = 50)
    {
        limit = Math.Clamp(limit, 1, 100);

        return await _dbContext.Notifications
            .AsNoTracking()
            .Where(x => x.RecipientUserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Take(limit)
            .Select(x => new NotificationResponseDto
            {
                Id = x.Id,
                ActorUserId = x.ActorUserId,
                Type = x.Type,
                Message = x.Message,
                CreatedAt = x.CreatedAt,
                IsRead = x.IsRead,
                WorkspaceId = x.WorkspaceId,
                EntityId = x.EntityId
            })
            .ToListAsync();
    }

    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(x =>
                x.Id == notificationId &&
                x.RecipientUserId == userId);

        if (notification is null)
        {
            return false;
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }

    public async Task<int> MarkAllAsReadAsync(Guid userId)
    {
        return await _dbContext.Notifications
            .Where(x =>
                x.RecipientUserId == userId &&
                !x.IsRead)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.IsRead, true));
    }

    private static NotificationResponseDto ToResponse(
        Notification notification)
    {
        return new NotificationResponseDto
        {
            Id = notification.Id,
            ActorUserId = notification.ActorUserId,
            Type = notification.Type,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead,
            WorkspaceId = notification.WorkspaceId,
            EntityId = notification.EntityId
        };
    }
}