using ChatApp.Application.Interfaces;
using ChatApp.Application.Realtime;
using ChatApp.Contracts.Realtime.Requests;
using ChatApp.Contracts.Realtime.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.RealTime.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IChannelAccessService _channelAccessService;

    private readonly IPresenceService _presenceService;

    private readonly IPrivateConversationService _privateConversationService;

    private readonly IWorkspaceMemberLookupService _workspaceMemberLookupService;

    private readonly IChatNotifier _chatNotifier;

    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IChannelAccessService channelAccessService,
        IPresenceService presenceService,
        IPrivateConversationService privateConversationService,
        IWorkspaceMemberLookupService workspaceMemberLookupService,
        IChatNotifier chatNotifier,
        ILogger<ChatHub> logger)
    {
        _channelAccessService = channelAccessService;
        _presenceService = presenceService;
        _privateConversationService = privateConversationService;
        _workspaceMemberLookupService = workspaceMemberLookupService;
        _chatNotifier = chatNotifier;
        _logger = logger;
    }

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

    public async Task LeavePrivateConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            SignalRGroups.PrivateConversationGroup(conversationId));
    }

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

    public async Task TypingStopped(
        TypingStoppedRequest request)
    {
        var userId = GetCurrentUserId();

        await _presenceService.TypingStoppedAsync(
            userId,
            request.ChannelId);
    }

    public async Task PrivateTypingStarted(
        Guid conversationId)
    {
        await BroadcastPrivateTypingAsync(conversationId, true);
    }

    public async Task PrivateTypingStopped(
        Guid conversationId)
    {
        await BroadcastPrivateTypingAsync(conversationId, false);
    }

    private async Task BroadcastPrivateTypingAsync(
        Guid conversationId,
        bool isTyping)
    {
        var userId = GetCurrentUserId();

        var isParticipant = await _privateConversationService
            .IsParticipantAsync(userId, conversationId);

        if (!isParticipant)
        {
            throw new HubException("Access denied");
        }

        var lookup = await _workspaceMemberLookupService
            .GetPresenceLookupAsync(userId);

        await _chatNotifier.PrivateUserTypingAsync(
            conversationId,
            new PrivateUserTypingResponseDto
            {
                ConversationId = conversationId,
                UserId = userId,
                Username = lookup.Username,
                IsTyping = isTyping
            });
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