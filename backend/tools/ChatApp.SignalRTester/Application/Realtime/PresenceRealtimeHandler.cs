using ChatApp.Contracts.Realtime;
using ChatApp.SignalRTester.Application.Startup;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Realtime;

public class PresenceRealtimeHandler : IApplicationInitializer
{
    private readonly ISignalRClient _signalRClient;

    private readonly OnlineUsersCache _onlineUsersCache;

    private readonly IConsoleOutput _consoleOutput;

    public PresenceRealtimeHandler(
        ISignalRClient signalRClient,
        OnlineUsersCache onlineUsersCache,
        IConsoleOutput consoleOutput)
    {
        _signalRClient = signalRClient;
        _onlineUsersCache = onlineUsersCache;
        _consoleOutput = consoleOutput;
    }

    public Task InitializeAsync()
    {
        _signalRClient.UserPresenceChanged +=
            OnUserPresenceChanged;

        _signalRClient.OnlineUsersSnapshot +=
            OnSnapshotReceived;

        return Task.CompletedTask;
    }

    private void OnUserPresenceChanged(
        UserPresenceChangedResponseDto response)
    {
        _onlineUsersCache.UserChanged(response);

        _consoleOutput.WriteUserPresenceChanged(
            response);
    }

    private void OnSnapshotReceived(
        IReadOnlyCollection<OnlineUserResponseDto> users)
    {
        _onlineUsersCache.SetUsers(users);
    }
}