using ChatApp.Contracts.Messages.Responses;
using ChatApp.SignalRTester.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.SignalR;

public class SignalRClient : ISignalRClient
{
    private readonly HubConnection _connection;

    public SignalRClient(
        IOptions<AppSettings> options)
    {
        _connection =
            new HubConnectionBuilder()
                .WithUrl(options.Value.HubUrl)
                .WithAutomaticReconnect()
                .Build();

        _connection.On<MessageResponseDto>(
            "MessageCreated",
            dto =>
            {
                MessageCreated?.Invoke(dto);
            });

        _connection.On<MessageResponseDto>(
            "MessageUpdated",
            dto =>
            {
                MessageUpdated?.Invoke(dto);
            });

        _connection.On<Guid>(
            "MessageDeleted",
            id =>
            {
                MessageDeleted?.Invoke(id);
            });
    }

    public bool IsConnected =>
        _connection.State == HubConnectionState.Connected;

    public event Action<MessageResponseDto>? MessageCreated;

    public event Action<MessageResponseDto>? MessageUpdated;

    public event Action<Guid>? MessageDeleted;

    public async Task ConnectAsync()
    {
        if (IsConnected)
        {
            return;
        }

        await _connection.StartAsync();
    }

    public async Task DisconnectAsync()
    {
        if (!IsConnected)
        {
            return;
        }

        await _connection.StopAsync();
    }

    public Task JoinChannelAsync(
        Guid channelId)
    {
        return _connection.InvokeAsync(
            "JoinChannel",
            channelId);
    }

    public Task LeaveChannelAsync(
        Guid channelId)
    {
        return _connection.InvokeAsync(
            "LeaveChannel",
            channelId);
    }
}