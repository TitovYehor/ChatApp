using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class SignalRWorkflow
{
    private readonly ISignalRClient _signalRClient;
    private readonly UserSession _userSession;
    private readonly IConsoleOutput _consoleOutput;

    public SignalRWorkflow(
        ISignalRClient signalRClient,
        UserSession userSession,
        IConsoleOutput consoleOutput)
    {
        _signalRClient = signalRClient;
        _userSession = userSession;
        _consoleOutput = consoleOutput;
    }

    public async Task ConnectAsync()
    {
        if (_signalRClient.IsConnected)
        {
            _consoleOutput.WriteInfo("Already connected");

            return;
        }

        await _signalRClient.ConnectAsync();

        if (_userSession.CurrentChannel != null)
        {
            await _signalRClient.JoinChannelAsync(
                _userSession.CurrentChannel.Id);
        }

        _consoleOutput.WriteSuccess("Connected to SignalR");
    }

    public async Task DisconnectAsync()
    {
        if (!_signalRClient.IsConnected)
        {
            _consoleOutput.WriteInfo("Not connected");

            return;
        }

        await _signalRClient.DisconnectAsync();

        _consoleOutput.WriteSuccess("Disconnected");
    }
}