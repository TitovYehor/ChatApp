using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Channels.Responses;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

/// <summary>
/// Broadcasts channel lifecycle events to the affected workspace members
/// </summary>
public sealed class SignalRChannelNotifier : IChannelNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRChannelNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc />
    public async Task ChannelCreatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        ChannelResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.ChannelCreated,
                response);
    }

    /// <inheritdoc />
    public async Task ChannelUpdatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        ChannelResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.ChannelUpdated,
                response);
    }

    /// <inheritdoc />
    public async Task ChannelDeletedAsync(
        IReadOnlyCollection<Guid> memberIds,
        ChannelDeletedResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.ChannelDeleted,
                response);
    }
}