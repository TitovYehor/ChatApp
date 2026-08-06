using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Workspaces.Responses;

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

    event Action<MessageDeletedResponseDto>? MessageDeleted;

    event Action<WorkspaceDeletedResponseDto>? WorkspaceDeleted;

    event Action<WorkspaceUpdatedResponseDto>? WorkspaceUpdated;

    event Action<UserPresenceChangedResponseDto>? UserPresenceChanged;

    event Action<IReadOnlyCollection<OnlineUserResponseDto>>? OnlineUsersSnapshot;

    event Action? Connected;

    event Action? Disconnected;

    event Func<Task>? Reconnected;
}