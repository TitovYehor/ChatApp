using ChatApp.Contracts.Notifications.Responses;

namespace ChatApp.Application.Interfaces;

public interface INotificationNotifier
{
    Task NotificationCreatedAsync(
        Guid recipientUserId,
        NotificationResponseDto notification);
}