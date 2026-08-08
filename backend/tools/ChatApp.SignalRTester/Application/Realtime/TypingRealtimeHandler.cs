using ChatApp.Contracts.Realtime.Responses;
using ChatApp.SignalRTester.Application.Startup;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Realtime;

public class TypingRealtimeHandler : IApplicationInitializer
{
    private readonly ISignalRClient _signalRClient;
    private readonly UserSession _userSession;
    private readonly IConsoleOutput _consoleOutput;

    public TypingRealtimeHandler(
        ISignalRClient signalRClient,
        UserSession userSession,
        IConsoleOutput consoleOutput)
    {
        _signalRClient = signalRClient;
        _userSession = userSession;
        _consoleOutput = consoleOutput;
    }

    public Task InitializeAsync()
    {
        _signalRClient.UserTyping += OnTyping;

        return Task.CompletedTask;
    }

    private void OnTyping(
        UserTypingResponseDto response)
    {
        if (_userSession.CurrentChannel?.Id != response.ChannelId)
        {
            return;
        }

        if (response.UserId == _userSession.UserId)
        {
            return;
        }

        _consoleOutput.WriteUserTyping(response);
    }
}