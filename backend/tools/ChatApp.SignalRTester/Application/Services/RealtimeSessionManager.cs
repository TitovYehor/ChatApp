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

        _signalRClient.Disconnected += OnDisconnected;
    }

    public bool IsConnected => _realtimeSession.IsConnected;

    public async Task ConnectAsync()
    {
        if (_realtimeSession.IsConnected)
        {
            return;
        }

        await _signalRClient.ConnectAsync();

        _realtimeSession.Connected();

        if (_userSession.CurrentChannel != null)
        {
            await _signalRClient.JoinChannelAsync(
                _userSession.CurrentChannel.Id);
        }
    }

    public async Task DisconnectAsync()
    {
        if (!_realtimeSession.IsConnected)
        {
            return;
        }

        await _signalRClient.DisconnectAsync();

        _realtimeSession.Disconnected();
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

    private void OnDisconnected()
    {
        _realtimeSession.Disconnected();
    }
}