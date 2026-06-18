using ChatApp.Application.DTOs.Messages;
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
    private readonly IMessageService _messageService;

    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IMessageService messageService,
        ILogger<ChatHub> logger)
    {
        _messageService = messageService;
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

    public async Task<MessageResponseDto> SendMessage(
        Guid channelId,
        CreateMessageRequestDto request)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation(
            "User {UserId} sent message to channel {ChannelId}",
            userId,
            channelId);

        return await _messageService.CreateAsync(
            channelId,
            userId,
            request);
    }

    public async Task<MessageResponseDto> UpdateMessage(
        Guid messageId,
        UpdateMessageRequestDto request)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation(
            "User {UserId} updating message {MessageId}",
            userId,
            messageId);

        return await _messageService.UpdateAsync(
            messageId,
            userId,
            request);
    }

    public async Task DeleteMessage(
        Guid messageId)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation(
            "User {UserId} deleting message {MessageId}",
            userId,
            messageId);

        await _messageService.DeleteAsync(
            messageId,
            userId);
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