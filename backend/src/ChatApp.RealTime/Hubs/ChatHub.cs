using ChatApp.Application.Realtime;
using ChatApp.Application.Realtime.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.RealTime.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        ILogger<ChatHub> logger)
    {
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
        _logger.LogInformation(
            "User {UserId} joined channel {ChannelId}",
            GetCurrentUserId(),
            request.ChannelId);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            SignalRGroups.Channel(
                request.ChannelId));
    }

    public async Task LeaveChannel(
        LeaveChannelRequest request)
    {
        _logger.LogInformation(
            "Connection {ConnectionId} left channel {ChannelId}",
            Context.ConnectionId,
            request.ChannelId);

        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            SignalRGroups.Channel(
                request.ChannelId));
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