using ChatApp.Contracts.Messages.Requests;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.Clients.Messages;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class MessageWorkflow
{
    private readonly IMessageApiClient _messageApiClient;

    private readonly UserSession _userSession;

    private readonly MessageCache _messageCache;

    private readonly ISignalRClient _signalRClient;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    public MessageWorkflow(
        IMessageApiClient messageApiClient,
        UserSession userSession,
        MessageCache messageCache,
        ISignalRClient signalRClient,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    {
        _messageApiClient = messageApiClient;
        _userSession = userSession;
        _messageCache = messageCache;
        _signalRClient = signalRClient;
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

        _consoleOutput.WriteMessageList(_messageCache.Messages);
    }

    public async Task SearchMessagesAsync()
    {
        if (_userSession.CurrentChannel == null)
        {
            _consoleOutput.WriteError("No channel selected");
            return;
        }

        _consoleOutput.WriteHeader("Search Messages");

        var phrase = _consoleInput.ReadRequiredString("Search");

        var query = new MessageQueryDto
        {
            PageNumber = 1,
            PageSize = 50,
            Search = phrase
        };

        var result = await _messageApiClient.GetByChannelAsync(
            _userSession.CurrentChannel.Id,
            query);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo($"Found {result.Data!.TotalCount} message(s)");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteMessageList(result.Data.Items);
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

    public async Task UpdateMessageAsync()
    {
        if (_userSession.CurrentChannel == null)
        {
            _consoleOutput.WriteError("No channel selected");
            return;
        }

        if (_messageCache.Messages.Count == 0)
        {
            _consoleOutput.WriteInfo("No messages loaded");
            return;
        }

        _consoleOutput.WriteHeader("Update Message");

        _consoleOutput.WriteMessageList(_messageCache.Messages);

        var selection = _consoleInput.ReadInt(
            "Select message",
            1,
            _messageCache.Messages.Count);

        var message = _messageCache.Messages[selection - 1];

        var content = _consoleInput.ReadRequiredString("New content");

        var request = new UpdateMessageRequestDto
        {
            Content = content
        };

        var result = await _messageApiClient.UpdateAsync(
            message.Id,
            request);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Message updated");

        _consoleOutput.WriteInfo("Waiting for realtime notification...");
    }

    public async Task DeleteMessageAsync()
    {
        if (_userSession.CurrentChannel == null)
        {
            _consoleOutput.WriteError("No channel selected");
            return;
        }

        if (_messageCache.Messages.Count == 0)
        {
            _consoleOutput.WriteInfo("No messages loaded");
            return;
        }

        _consoleOutput.WriteHeader("Delete Message");

        _consoleOutput.WriteMessageList(_messageCache.Messages);

        var selection = _consoleInput.ReadInt(
            "Select message",
            1,
            _messageCache.Messages.Count);

        var message = _messageCache.Messages[selection - 1];

        var confirmed = _consoleInput.ReadConfirmation("Delete selected message?");

        if (!confirmed)
        {
            _consoleOutput.WriteInfo("Operation cancelled");
            return;
        }

        var result = await _messageApiClient.DeleteAsync(
            message.Id);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Message deleted");

        _consoleOutput.WriteInfo("Waiting for realtime notification...");
    }

    public async Task StartTypingAsync()
    {
        if (_userSession.CurrentChannel == null)
        {
            _consoleOutput.WriteError("Select a channel first");
            return;
        }

        if (!_signalRClient.IsConnected)
        {
            _consoleOutput.WriteError("SignalR is not connected");
            return;
        }

        await _signalRClient.TypingStartedAsync(
            _userSession.CurrentChannel.Id);

        _consoleOutput.WriteSuccess("Typing started");
    }

    public async Task StopTypingAsync()
    {
        if (_userSession.CurrentChannel == null)
        {
            _consoleOutput.WriteError("Select a channel first");
            return;
        }

        if (!_signalRClient.IsConnected)
        {
            _consoleOutput.WriteError("SignalR is not connected");
            return;
        }

        await _signalRClient.TypingStoppedAsync(
            _userSession.CurrentChannel.Id);

        _consoleOutput.WriteSuccess("Typing stopped");
    }
}