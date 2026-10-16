using ChatApp.Contracts.Notifications.Responses;

namespace ChatApp.Application.Interfaces;

public interface INotificationService
{
    Task<NotificationResponseDto> CreateAsync(
        Guid recipientUserId,
        Guid? actorUserId,
        string type,
        string message,
        Guid? workspaceId = null,
        Guid? entityId = null);

    Task<IReadOnlyList<NotificationResponseDto>> GetLatestAsync(
        Guid userId,
        int limit = 50);

    Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId);

    Task<int> MarkAllAsReadAsync(Guid userId);
}