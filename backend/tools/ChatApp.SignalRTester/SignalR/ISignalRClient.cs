using ChatApp.Contracts.Messages.Responses;

namespace ChatApp.SignalRTester.SignalR;

public interface ISignalRClient
{
    bool IsConnected { get; }

    Task ConnectAsync();

    Task DisconnectAsync();

    Task JoinChannelAsync(
        Guid channelId);

    Task LeaveChannelAsync(
        Guid channelId);

    event Action<MessageResponseDto>? MessageCreated;

    event Action<MessageResponseDto>? MessageUpdated;

    event Action<Guid>? MessageDeleted;

    event Action? Connected;

    event Action? Disconnected;

    event Func<Task>? Reconnected;
}