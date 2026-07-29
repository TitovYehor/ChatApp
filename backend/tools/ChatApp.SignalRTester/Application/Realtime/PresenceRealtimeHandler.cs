using ChatApp.Contracts.Realtime;
using ChatApp.SignalRTester.Application.Startup;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Realtime;

public class PresenceRealtimeHandler
    : IApplicationInitializer
{
    private readonly ISignalRClient _signalRClient;

    private readonly IConsoleOutput _consoleOutput;

    public PresenceRealtimeHandler(
        ISignalRClient signalRClient,
        IConsoleOutput consoleOutput)
    {
        _signalRClient = signalRClient;
        _consoleOutput = consoleOutput;
    }

    public Task InitializeAsync()
    {
        _signalRClient.UserPresenceChanged +=
            OnUserPresenceChanged;

        return Task.CompletedTask;
    }

    private void OnUserPresenceChanged(
        UserPresenceChangedResponseDto response)
    {
        _consoleOutput.WriteUserPresenceChanged(
            response);
    }
}