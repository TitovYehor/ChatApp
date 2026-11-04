using ChatApp.Contracts.Notifications.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Creates, retrieves, and manages user notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Creates a notification for the specified recipient
    /// </summary>
    Task<NotificationResponseDto> CreateAsync(
        Guid recipientUserId,
        Guid? actorUserId,
        string type,
        string message,
        Guid? workspaceId = null,
        Guid? entityId = null);

    /// <summary>
    /// Gets the latest notifications for a user, up to the specified limit
    /// </summary>
    Task<IReadOnlyList<NotificationResponseDto>> GetLatestAsync(
        Guid userId,
        int limit = 50);

    /// <summary>
    /// Marks a notification as read for the specified user
    /// </summary>
    Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId);

    /// <summary>
    /// Marks all notifications as read for the specified user
    /// </summary>
    Task<int> MarkAllAsReadAsync(
        Guid userId);
}