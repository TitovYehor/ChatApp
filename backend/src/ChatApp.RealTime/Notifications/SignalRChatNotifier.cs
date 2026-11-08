using ChatApp.Contracts.Messages.Responses;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Realtime;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.Contracts.PrivateConversations.Responses;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

/// <summary>
/// Broadcasts chat messages, presence updates, and typing events to SignalR groups
/// </summary>
public sealed class SignalRChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRChatNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task PrivateMessageCreatedAsync(
        Guid conversationId,
        PrivateMessageResponseDto message)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.PrivateConversationGroup(conversationId))
            .SendAsync(
                SignalREvents.PrivateMessageCreated,
                message);
    }

    /// <inheritdoc />
    public async Task PrivateMessageUpdatedAsync(
        Guid conversationId,
        PrivateMessageResponseDto message)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.PrivateConversationGroup(conversationId))
            .SendAsync(
                SignalREvents.PrivateMessageUpdated,
                message);
    }

    /// <inheritdoc />
    public async Task PrivateMessageDeletedAsync(
        Guid conversationId,
        PrivateMessageDeletedResponseDto response)
    {
        await _hubContext.Clients
            .Group(
                SignalRGroups.PrivateConversationGroup(conversationId))
            .SendAsync(
                SignalREvents.PrivateMessageDeleted,
                response);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Task PrivateUserTypingAsync(
        Guid conversationId,
        PrivateUserTypingResponseDto response)
    {
        return _hubContext.Clients
            .Group(SignalRGroups.PrivateConversationGroup(conversationId))
            .SendAsync(
                SignalREvents.PrivateUserTyping,
                response);
    }
}