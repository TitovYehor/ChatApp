using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

public sealed class SignalRFriendshipNotifier
    : IFriendshipNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRFriendshipNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task FriendshipChangedAsync(
        IReadOnlyCollection<Guid> userIds)
    {
        var recipients = userIds
            .Distinct()
            .Select(id => id.ToString())
            .ToArray();

        await _hubContext.Clients
            .Users(recipients)
            .SendAsync(SignalREvents.FriendshipChanged);
    }
}