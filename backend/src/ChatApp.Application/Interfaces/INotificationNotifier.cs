using ChatApp.Contracts.Notifications.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines realtime delivery of user notifications
/// </summary>
public interface INotificationNotifier
{
    /// <summary>
    /// Delivers a newly created notification to its recipient
    /// </summary>
    Task NotificationCreatedAsync(
        Guid recipientUserId,
        NotificationResponseDto notification);
}