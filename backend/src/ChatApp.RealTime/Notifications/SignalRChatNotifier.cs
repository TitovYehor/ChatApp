using ChatApp.Contracts.Messages.Responses;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Realtime;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Realtime.SignalRNamings;
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
                SignalRGroups.ChannelGroup(channelId))
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
                SignalRGroups.ChannelGroup(channelId))
            .SendAsync(
                SignalREvents.MessageUpdated,
                message);
    }

    public async Task MessageDeletedAsync(
        Guid channelId,
        MessageDeletedResponseDto response)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.ChannelGroup(channelId))
            .SendAsync(
                SignalREvents.MessageDeleted,
                response);
    }

    public async Task UserPresenceChangedAsync(
        IEnumerable<Guid> userIds,
        UserPresenceChangedResponseDto response)
    {
        await _hubContext.Clients
            .Groups(
                userIds.Select(SignalRGroups.UserGroup))
            .SendAsync(
                SignalREvents.UserPresenceChanged,
                response);
    }

    public async Task OnlineUsersSnapshotAsync(
        Guid userId,
        IReadOnlyCollection<OnlineUserResponseDto> users)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.UserGroup(userId))
            .SendAsync(
                SignalREvents.OnlineUsersSnapshot,
                users);
    }

    public Task UserTypingAsync(
        Guid channelId,
        UserTypingResponseDto response)
    {
        return _hubContext.Clients
            .Group(SignalRGroups.ChannelGroup(channelId))
            .SendAsync(
                SignalREvents.UserTyping,
                response);
    }
}