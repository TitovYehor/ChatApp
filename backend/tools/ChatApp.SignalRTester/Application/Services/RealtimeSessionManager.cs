using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;

namespace ChatApp.SignalRTester.Application.Services;

public class RealtimeSessionManager
{
    private readonly ISignalRClient _signalRClient;

    private readonly RealtimeSession _realtimeSession;

    private readonly UserSession _userSession;

    public RealtimeSessionManager(
        ISignalRClient signalRClient,
        RealtimeSession realtimeSession,
        UserSession userSession)
    {
        _signalRClient = signalRClient;
        _realtimeSession = realtimeSession;
        _userSession = userSession;

        _signalRClient.Connected += OnConnected;

        _signalRClient.Disconnected += OnDisconnected;

        _signalRClient.Reconnected += OnReconnected;
    }

    public bool IsConnected => _signalRClient.IsConnected;

    public async Task ConnectAsync()
    {
        if (_signalRClient.IsConnected)
        {
            return;
        }

        await _signalRClient.ConnectAsync();

        if (_userSession.CurrentChannel != null)
        {
            await _signalRClient.JoinChannelAsync(
                _userSession.CurrentChannel.Id);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_signalRClient.IsConnected)
        {
            await _signalRClient.DisconnectAsync();
        }

        _realtimeSession.MarkDisconnected();
    }

    public async Task ChangeChannelAsync(
        Guid? previousChannelId,
        Guid newChannelId)
    {
        await ConnectAsync();

        if (previousChannelId.HasValue)
        {
            await _signalRClient.LeaveChannelAsync(
                previousChannelId.Value);
        }

        await _signalRClient.JoinChannelAsync(
            newChannelId);
    }

    public async Task LeaveChannelAsync(
        Guid channelId)
    {
        if (!_signalRClient.IsConnected)
        {
            return;
        }

        await _signalRClient.LeaveChannelAsync(channelId);
    }

    private void OnConnected()
    {
        _realtimeSession.MarkConnected();
    }

    private void OnDisconnected()
    {
        _realtimeSession.MarkDisconnected();
    }

    private async Task OnReconnected()
    {
        try
        {
            _realtimeSession.MarkConnected();

            if (_userSession.CurrentChannel == null)
            {
                return;
            }

            await _signalRClient.JoinChannelAsync(
                _userSession.CurrentChannel.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to restore realtime channel: {ex.Message}");
        }
    }
}