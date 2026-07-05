using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.SignalRTester.Clients.Messages;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class MessageWorkflow
{
    private readonly IMessageApiClient _messageApiClient;

    private readonly UserSession _userSession;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    private readonly List<MessageResponseDto> _loadedMessages = [];

    public MessageWorkflow(
        IMessageApiClient messageApiClient,
        UserSession userSession,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    {
        _messageApiClient = messageApiClient;
        _userSession = userSession;
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

        var result = await _messageApiClient.GetByChannelAsync(
            _userSession.CurrentChannel.Id,
            query);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _loadedMessages.Clear();

        _loadedMessages.AddRange(result.Data!.Items);

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo($"Loaded {_loadedMessages.Count} message(s)");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteMessageList(_loadedMessages);
    }
}