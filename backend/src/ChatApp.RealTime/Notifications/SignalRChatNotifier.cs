using ChatApp.Application.DTOs.Messages;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Realtime;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

public sealed class SignalRChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRChatNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task MessageCreatedAsync(
        Guid channelId,
        MessageResponseDto message)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.Channel(channelId))
            .SendAsync(
                SignalREvents.MessageCreated,
                message);
    }

    public async Task MessageUpdatedAsync(
        Guid channelId,
        MessageResponseDto message)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.Channel(channelId))
            .SendAsync(
                SignalREvents.MessageUpdated,
                message);
    }

    public async Task MessageDeletedAsync(
        Guid channelId,
        Guid messageId)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.Channel(channelId))
            .SendAsync(
                SignalREvents.MessageDeleted,
                messageId);
    }
}