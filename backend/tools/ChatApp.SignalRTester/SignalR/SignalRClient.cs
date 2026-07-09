using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Realtime;
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

    public event Action<Guid>? MessageDeleted;

    public event Action? Disconnected;

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

    private void RegisterLifecycleEvents()
    {
        _connection.Closed += OnClosedAsync;
    }

    private Task OnClosedAsync(
        Exception? exception)
    {
        Disconnected?.Invoke();

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