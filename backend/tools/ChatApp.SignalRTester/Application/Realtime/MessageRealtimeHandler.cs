using ChatApp.Contracts.Messages.Responses;
using ChatApp.SignalRTester.Application.Startup;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Realtime;

public class MessageRealtimeHandler : IApplicationInitializer
{
    private readonly ISignalRClient _signalRClient;

    private readonly MessageCache _messageCache;

    private readonly UserSession _userSession;

    private readonly IConsoleOutput _consoleOutput;

    public MessageRealtimeHandler(
        ISignalRClient signalRClient,
        MessageCache messageCache,
        UserSession userSession,
        IConsoleOutput consoleOutput)
    {
        _signalRClient = signalRClient;
        _messageCache = messageCache;
        _userSession = userSession;
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
        if (_userSession.CurrentChannel?.Id != message.ChannelId)
        {
            return;
        }

        _messageCache.Add(message);

        _consoleOutput.WriteRealtimeMessageCreated(message);
    }

    private void OnMessageUpdated(
        MessageResponseDto message)
    {
        if (_userSession.CurrentChannel?.Id != message.ChannelId)
        {
            return;
        }

        _messageCache.Update(message);

        _consoleOutput.WriteRealtimeMessageUpdated(message);
    }

    private void OnMessageDeleted(
        Guid messageId)
    {
        _messageCache.Remove(messageId);

        _consoleOutput.WriteRealtimeMessageDeleted(messageId);
    }
}