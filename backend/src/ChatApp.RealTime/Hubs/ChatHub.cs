using ChatApp.Application.Realtime;
using ChatApp.Application.Realtime.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected: {Context.ConnectionId}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        Console.WriteLine($"Disconnected: {Context.ConnectionId}");

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChannel(
        JoinChannelRequest request)
    {
        Console.WriteLine(
            $"Connection {Context.ConnectionId} joined channel {request.ChannelId}");

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            SignalRGroups.Channel(
                request.ChannelId));
    }

    public async Task LeaveChannel(
        LeaveChannelRequest request)
    {
        Console.WriteLine(
            $"Connection {Context.ConnectionId} left channel {request.ChannelId}");

        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            SignalRGroups.Channel(
                request.ChannelId));
    }
}