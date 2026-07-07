using ChatApp.Contracts.Messages.Requests;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.Clients.Messages;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class MessageWorkflow
{
    private readonly IMessageApiClient _messageApiClient;

    private readonly UserSession _userSession;

    private readonly MessageCache _messageCache;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    public MessageWorkflow(
        IMessageApiClient messageApiClient,
        UserSession userSession,
        MessageCache messageCache,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    {
        _messageApiClient = messageApiClient;
        _userSession = userSession;
        _messageCache = messageCache;
        _consoleInput = consoleInput;
        _consoleOutput = consoleOutput;
    }

    public async Task LoadMessagesAsync()
    {
        if (_userSession.CurrentChannel == null)
        {
            _consoleOutput.WriteError("No channel selected");
            return;
        }

        _consoleOutput.WriteHeader("Load Messages");

        var query = new MessageQueryDto
        {
            PageNumber = 1,
            PageSize = 50
        };

        _consoleOutput.WriteInfo("Loading messages...");

        var result = await _messageApiClient.GetByChannelAsync(
            _userSession.CurrentChannel.Id,
            query);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteSeparator();
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var messages = result.Data!.Items;

        _messageCache.Replace(messages);

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo($"Loaded {messages.Count} message(s)");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteMessageList(messages);
    }

    public async Task SendMessageAsync()
    {
        if (_userSession.CurrentChannel == null)
        {
            _consoleOutput.WriteError("No channel selected");
            return;
        }

        _consoleOutput.WriteHeader("Send Message");

        var content = _consoleInput.ReadRequiredString(
            "Message");

        var request = new CreateMessageRequestDto
        {
            Content = content
        };

        var result = await _messageApiClient.CreateAsync(
            _userSession.CurrentChannel.Id,
            request);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Message sent");

        _consoleOutput.WriteInfo("Waiting for realtime notification...");
    }
}