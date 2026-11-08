using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Notifications.Responses;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

/// <summary>
/// Delivers newly created notifications to their intended recipients in realtime
/// </summary>
public sealed class SignalRNotificationNotifier : INotificationNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRNotificationNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc />
    public async Task NotificationCreatedAsync(
        Guid recipientUserId,
        NotificationResponseDto notification)
    {
        await _hubContext.Clients
            .Users(new[] { recipientUserId.ToString() })
            .SendAsync(
                SignalREvents.NotificationCreated,
                notification);
    }
}