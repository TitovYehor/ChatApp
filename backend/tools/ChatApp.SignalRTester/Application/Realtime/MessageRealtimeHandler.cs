using ChatApp.Contracts.Messages.Responses;
using ChatApp.SignalRTester.Application.Startup;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Realtime;

public class MessageRealtimeHandler : IApplicationInitializer
{
    private readonly ISignalRClient _signalRClient;

    private readonly MessageCache _messageCache;

    private readonly IConsoleOutput _consoleOutput;

    public MessageRealtimeHandler(
        ISignalRClient signalRClient,
        MessageCache messageCache,
        IConsoleOutput consoleOutput)
    {
        _signalRClient = signalRClient;
        _messageCache = messageCache;
        _consoleOutput = consoleOutput;
    }

    public Task InitializeAsync()
    {
        _signalRClient.MessageCreated += OnMessageCreated;

        _signalRClient.MessageUpdated += OnMessageUpdated;

        _signalRClient.MessageDeleted += OnMessageDeleted;

        return Task.CompletedTask;
    }

    private void OnMessageCreated(
        MessageResponseDto message)
    {
        _messageCache.Add(message);

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteSuccess("Realtime event: Message created");

        _consoleOutput.WriteMessage(message);

        _consoleOutput.WriteSeparator();
    }

    private void OnMessageUpdated(
        MessageResponseDto message)
    {
        _messageCache.Update(message);

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo("Realtime event: Message updated");

        _consoleOutput.WriteMessage(message);

        _consoleOutput.WriteSeparator();
    }

    private void OnMessageDeleted(
        Guid messageId)
    {
        _messageCache.Remove(messageId);

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo($"Realtime event: Message deleted ({messageId})");

        _consoleOutput.WriteSeparator();
    }
}