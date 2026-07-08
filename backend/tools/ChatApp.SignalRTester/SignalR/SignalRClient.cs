using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Realtime;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.Session.AuthenticationState;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.SignalR;

public class SignalRClient : ISignalRClient
{
    private readonly HubConnection _connection;

    private readonly RealtimeSession _realtimeSession;

    private readonly IAccessTokenProvider _tokenProvider;

    public SignalRClient(
        IOptions<AppSettings> options,
        RealtimeSession realtimeSession,
        IAccessTokenProvider tokenProvider)
    {
        _realtimeSession = realtimeSession;

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

    public event Action<Guid>? MessageDeleted;

    public async Task ConnectAsync()
    {
        if (IsConnected)
        {
            return;
        }

        await _connection.StartAsync();

        _realtimeSession.Connected();
    }

    public async Task DisconnectAsync()
    {
        if (!IsConnected)
        {
            return;
        }

        await _connection.StopAsync();

        _realtimeSession.Disconnected();
    }

    public async Task JoinChannelAsync(
        Guid channelId)
    {
        await _connection.InvokeAsync(
            SignalRMethods.JoinChannel,
            new JoinChannelRequest
            {
                ChannelId = channelId
            });

        _realtimeSession.JoinedChannel(channelId);
    }

    public async Task LeaveChannelAsync(
        Guid channelId)
    {
        await _connection.InvokeAsync(
            SignalRMethods.LeaveChannel,
            new LeaveChannelRequest
            {
                ChannelId = channelId
            });

        _realtimeSession.LeftChannel();
    }

    private void RegisterLifecycleEvents()
    {
        _connection.Closed += OnClosedAsync;
    }

    private Task OnClosedAsync(
        Exception? exception)
    {
        _realtimeSession.Disconnected();

        return Task.CompletedTask;
    }

    private void RegisterMessageEvents()
    {
        _connection.On<MessageResponseDto>(
            SignalREvents.MessageCreated,
            RaiseMessageCreated);

        _connection.On<MessageResponseDto>(
            SignalREvents.MessageUpdated,
            RaiseMessageUpdated);

        _connection.On<Guid>(
            SignalREvents.MessageDeleted,
            RaiseMessageDeleted);
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
        Guid guid)
    {
        MessageDeleted?.Invoke(guid);
    }
}