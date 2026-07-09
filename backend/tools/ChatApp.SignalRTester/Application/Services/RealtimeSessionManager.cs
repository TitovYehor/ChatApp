using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;

namespace ChatApp.SignalRTester.Application.Services;

public class RealtimeSessionManager
{
    private readonly ISignalRClient _signalRClient;

    private readonly UserSession _session;

    public RealtimeSessionManager(
        ISignalRClient signalRClient,
        UserSession session)
    {
        _signalRClient = signalRClient;
        _session = session;
    }

    public async Task ConnectAsync()
    {
        if (_signalRClient.IsConnected)
        {
            return;
        }

        await _signalRClient.ConnectAsync();

        if (_session.CurrentChannel != null)
        {
            await _signalRClient.JoinChannelAsync(
                _session.CurrentChannel.Id);
        }
    }

    public async Task DisconnectAsync()
    {
        if (!_signalRClient.IsConnected)
        {
            return;
        }

        await _signalRClient.DisconnectAsync();
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
}