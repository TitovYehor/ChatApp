using ChatApp.Application.Interfaces;
using ChatApp.Application.Realtime;
using ChatApp.Contracts.Realtime.Requests;
using ChatApp.Contracts.Realtime.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.RealTime.Hubs;

/// <summary>
/// Handles authenticated SignalR connections and realtime chat interactions
/// </summary>
[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IChannelAccessService _channelAccessService;

    private readonly IPresenceService _presenceService;

    private readonly IPrivateConversationService _privateConversationService;

    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IChannelAccessService channelAccessService,
        IPresenceService presenceService,
        IPrivateConversationService privateConversationService,
        ILogger<ChatHub> logger)
    {
        _channelAccessService = channelAccessService;
        _presenceService = presenceService;
        _privateConversationService = privateConversationService;
        _logger = logger;
    }

    /// <summary>
    /// Adds the connection to the user's group and updates their online presence
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            SignalRGroups.UserGroup(userId));

        await _presenceService.UserConnectedAsync(
            userId,
            Context.ConnectionId);

        _logger.LogInformation(
            "Connection established. ConnectionId: {ConnectionId}. UserId: {UserId}",
            Context.ConnectionId,
            Context.UserIdentifier);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Removes the connection from the user's group and updates their online presence
    /// </summary>
    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        if (Context.UserIdentifier != null)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                SignalRGroups.UserGroup(GetCurrentUserId()));

            await _presenceService.UserDisconnectedAsync(
                Guid.Parse(Context.UserIdentifier),
                Context.ConnectionId);
        }

        _logger.LogInformation(
            "Connection closed. ConnectionId: {ConnectionId}. UserId: {UserId}",
            Context.ConnectionId,
            Context.UserIdentifier);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Adds the current connection to a channel group after verifying channel access
    /// </summary>
    public async Task JoinChannel(
        JoinChannelRequest request)
    {
        var userId = GetCurrentUserId();

        var hasAccess = await _channelAccessService
            .CanAccessChannelAsync(
                userId,
                request.ChannelId);

        if (!hasAccess)
        {
            throw new HubException("Access denied");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            SignalRGroups.ChannelGroup(
                request.ChannelId));

        _logger.LogInformation(
            "User {UserId} joined channel {ChannelId}",
            GetCurrentUserId(),
            request.ChannelId);
    }

    /// <summary>
    /// Removes the current connection from a channel group
    /// </summary>
    public async Task LeaveChannel(
        LeaveChannelRequest request)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            SignalRGroups.ChannelGroup(
                request.ChannelId));

        _logger.LogInformation(
            "Connection {ConnectionId} left channel {ChannelId}",
            Context.ConnectionId,
            request.ChannelId);
    }

    /// <summary>
    /// Adds the current connection to a private conversation group after verifying participation
    /// </summary>
    public async Task JoinPrivateConversation(
        Guid conversationId)
    {
        var userId = GetCurrentUserId();

        var isParticipant = await _privateConversationService
            .IsParticipantAsync(userId, conversationId);

        if (!isParticipant)
        {
            throw new HubException("Access denied");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            SignalRGroups.PrivateConversationGroup(conversationId));

        _logger.LogInformation(
            "User {UserId} joined private conversation {ConversationId}",
            userId,
            conversationId);
    }

    /// <summary>
    /// Removes the current connection from a private conversation group
    /// </summary>
    public async Task LeavePrivateConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            SignalRGroups.PrivateConversationGroup(conversationId));
    }

    /// <summary>
    /// Notifies channel participants that the current user has started typing
    /// </summary>
    public async Task TypingStarted(
        TypingStartedRequest request)
    {
        var userId = GetCurrentUserId();

        var hasAccess = await _channelAccessService
            .CanAccessChannelAsync(
                userId,
                request.ChannelId);

        if (!hasAccess)
        {
            throw new HubException("Access denied");
        }

        await _presenceService.TypingStartedAsync(
            userId,
            request.ChannelId);
    }

    /// <summary>
    /// Notifies channel participants that the current user has stopped typing
    /// </summary>
    public async Task TypingStopped(
        TypingStoppedRequest request)
    {
        var userId = GetCurrentUserId();

        await _presenceService.TypingStoppedAsync(
            userId,
            request.ChannelId);
    }

    /// <summary>
    /// Notifies private conversation participants that the current user has started typing
    /// </summary>
    public async Task PrivateTypingStarted(Guid conversationId)
    {
        var userId = GetCurrentUserId();

        var isParticipant = await _privateConversationService
            .IsParticipantAsync(userId, conversationId);

        if (!isParticipant)
        {
            throw new HubException("Access denied");
        }

        await _presenceService.PrivateTypingStartedAsync(
            userId,
            conversationId);
    }

    /// <summary>
    /// Notifies private conversation participants that the current user has stopped typing
    /// </summary>
    public async Task PrivateTypingStopped(Guid conversationId)
    {
        var userId = GetCurrentUserId();

        var isParticipant = await _privateConversationService
            .IsParticipantAsync(userId, conversationId);

        if (!isParticipant)
        {
            throw new HubException("Access denied");
        }

        await _presenceService.PrivateTypingStoppedAsync(
            userId,
            conversationId);
    }

    private Guid GetCurrentUserId()
    {
        if (Context.UserIdentifier is null)
        {
            throw new HubException("User is not authenticated");
        }

        return Guid.Parse(Context.UserIdentifier);
    }
}