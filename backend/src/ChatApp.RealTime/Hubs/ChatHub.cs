using ChatApp.Application.Interfaces;
using ChatApp.Application.Realtime;
using ChatApp.Application.Realtime.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.RealTime.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IChannelAccessService _channelAccessService;

    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IChannelAccessService channelAccessService,
        ILogger<ChatHub> logger)
    {
        _channelAccessService = channelAccessService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "Connection established. ConnectionId: {ConnectionId}. UserId: {UserId}",
            Context.ConnectionId,
            Context.UserIdentifier);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
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
            SignalRGroups.Channel(
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
            SignalRGroups.Channel(
                request.ChannelId));

        _logger.LogInformation(
            "Connection {ConnectionId} left channel {ChannelId}",
            Context.ConnectionId,
            request.ChannelId);
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