using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Channels.Responses;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

public sealed class SignalRChannelNotifier : IChannelNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRChannelNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

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