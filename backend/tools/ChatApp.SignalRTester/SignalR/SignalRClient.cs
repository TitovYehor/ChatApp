using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Realtime.Requests;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Session.AuthenticationState;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.SignalR;

public class SignalRClient : ISignalRClient
{
    private readonly HubConnection _connection;

    private readonly IAccessTokenProvider _tokenProvider;

    public SignalRClient(
        IOptions<AppSettings> options,
        IAccessTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;

        _connection = new HubConnectionBuilder()
            .WithUrl(
                options.Value.HubUrl,
                configuration =>
                {
                    configuration.AccessTokenProvider = () =>
                        Task.FromResult(
                            _tokenProvider.GetToken());
                })
            .WithAutomaticReconnect()
            .Build();

        RegisterLifecycleEvents();

        RegisterMessageEvents();
    }

    public bool IsConnected => _connection.State == HubConnectionState.Connected;

    public event Action<MessageResponseDto>? MessageCreated;

    public event Action<MessageResponseDto>? MessageUpdated;

    public event Action<MessageDeletedResponseDto>? MessageDeleted;

    public event Action<WorkspaceDeletedResponseDto>? WorkspaceDeleted;

    public event Action<WorkspaceUpdatedResponseDto>? WorkspaceUpdated;

    public event Action<UserPresenceChangedResponseDto>? UserPresenceChanged;

    public event Action<IReadOnlyCollection<OnlineUserResponseDto>>? OnlineUsersSnapshot;

    public event Action<UserTypingResponseDto>? UserTyping;

    public event Action? Connected;

    public event Action? Disconnected;

    public event Func<Task>? Reconnected;

    public async Task ConnectAsync()
    {
        if (IsConnected)
        {
            return;
        }

        await _connection.StartAsync();

        Connected?.Invoke();
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
            SignalRMethods.JoinChannel,
            new JoinChannelRequest
            {
                ChannelId = channelId
            });
    }

    public Task LeaveChannelAsync(
        Guid channelId)
    {
        return _connection.InvokeAsync(
            SignalRMethods.LeaveChannel,
            new LeaveChannelRequest
            {
                ChannelId = channelId
            });
    }

    public Task TypingStartedAsync(
        Guid channelId)
    {
        return _connection.InvokeAsync(
            SignalRMethods.TypingStarted,
            new TypingStartedRequest
            {
                ChannelId = channelId
            });
    }

    public Task TypingStoppedAsync(
        Guid channelId)
    {
        return _connection.InvokeAsync(
            SignalRMethods.TypingStopped,
            new TypingStoppedRequest
            {
                ChannelId = channelId
            });
    }

    private void RegisterLifecycleEvents()
    {
        _connection.Closed += OnClosedAsync;

        _connection.Reconnecting += error =>
        {
            Disconnected?.Invoke();

            return Task.CompletedTask;
        };

        _connection.Reconnected += OnReconnectedAsync;
    }

    private Task OnClosedAsync(
        Exception? exception)
    {
        Disconnected?.Invoke();

        return Task.CompletedTask;
    }

    private async Task OnReconnectedAsync(
        string? connectionId)
    {
        if (Reconnected != null)
        {
            await Reconnected.Invoke();
        }
    }

    private void RegisterMessageEvents()
    {
        _connection.On<MessageResponseDto>(
            SignalREvents.MessageCreated,
            RaiseMessageCreated);

        _connection.On<MessageResponseDto>(
            SignalREvents.MessageUpdated,
            RaiseMessageUpdated);

        _connection.On<MessageDeletedResponseDto>(
            SignalREvents.MessageDeleted,
            RaiseMessageDeleted);

        _connection.On<WorkspaceDeletedResponseDto>(
            SignalREvents.WorkspaceDeleted,
            RaiseWorkspaceDeleted);

        _connection.On<WorkspaceUpdatedResponseDto>(
            SignalREvents.WorkspaceUpdated,
            RaiseWorkspaceUpdated);

        _connection.On<UserPresenceChangedResponseDto>(
            SignalREvents.UserPresenceChanged,
            RaiseUserPresenceChanged);

        _connection.On<IReadOnlyCollection<OnlineUserResponseDto>>(
            SignalREvents.OnlineUsersSnapshot,
            RaiseOnlineUsersSnapshot);

        _connection.On<UserTypingResponseDto>(
            SignalREvents.UserTyping,
            RaiseUserTyping);
    }

    private void RaiseMessageCreated(
        MessageResponseDto message)
    {
        MessageCreated?.Invoke(message);
    }

    private void RaiseMessageUpdated(
        MessageResponseDto message)
    {
        MessageUpdated?.Invoke(message);
    }

    private void RaiseMessageDeleted(
        MessageDeletedResponseDto response)
    {
        MessageDeleted?.Invoke(response);
    }

    private void RaiseWorkspaceDeleted(
        WorkspaceDeletedResponseDto response)
    {
        WorkspaceDeleted?.Invoke(response);
    }

    private void RaiseWorkspaceUpdated(
        WorkspaceUpdatedResponseDto response)
    {
        WorkspaceUpdated?.Invoke(response);
    }

    private void RaiseUserPresenceChanged(
        UserPresenceChangedResponseDto response)
    {
        UserPresenceChanged?.Invoke(response);
    }

    private void RaiseOnlineUsersSnapshot(
        IReadOnlyCollection<OnlineUserResponseDto> users)
    {
        OnlineUsersSnapshot?.Invoke(users);
    }

    private void RaiseUserTyping(
        UserTypingResponseDto response)
    {
        UserTyping?.Invoke(response);
    }
}