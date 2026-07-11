using ChatApp.SignalRTester.Application.Services;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class SignalRWorkflow
{
    private readonly RealtimeSessionManager _realtimeSessionManager;
    private readonly IConsoleOutput _consoleOutput;

    public SignalRWorkflow(
        RealtimeSessionManager realtimeSessionManager,
        IConsoleOutput consoleOutput)
    {
        _realtimeSessionManager = realtimeSessionManager;
        _consoleOutput = consoleOutput;
    }

    public async Task ConnectAsync()
    {
        if (_realtimeSessionManager.IsConnected)
        {
            _consoleOutput.WriteInfo("Already connected");
            return;
        }

        try
        {
            await _realtimeSessionManager.ConnectAsync();

            _consoleOutput.WriteSuccess("Connected to SignalR");
        }
        catch (Exception ex)
        {
            _consoleOutput.WriteError($"SignalR connection failed: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        if (!_realtimeSessionManager.IsConnected)
        {
            _consoleOutput.WriteInfo("Not connected");
            return;
        }

        try
        {
            await _realtimeSessionManager.DisconnectAsync();

            _consoleOutput.WriteSuccess("Disconnected");
        }
        catch (Exception ex)
        {
            _consoleOutput.WriteError($"SignalR disconnection failed: {ex.Message}");
        }
    }
}