using ChatApp.Contracts.Channels.Requests;
using ChatApp.SignalRTester.Application.Services;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.Clients.Channels;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class ChannelWorkflow
{
    private readonly IChannelApiClient _channelApiClient;

    private readonly RealtimeSessionManager _realtimeSessionManager;

    private readonly UserSession _userSession;

    private readonly MessageWorkflow _messageWorkflow;

    private readonly MessageCache _messageCache;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    public ChannelWorkflow(
        IChannelApiClient channelApiClient,
        RealtimeSessionManager realtimeSessionManager,
        UserSession userSession,
        MessageWorkflow messageWorkflow,
        MessageCache messageCache,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    {
        _channelApiClient = channelApiClient;
        _realtimeSessionManager = realtimeSessionManager;
        _userSession = userSession;
        _messageWorkflow = messageWorkflow;
        _messageCache = messageCache;
        _consoleInput = consoleInput;
        _consoleOutput = consoleOutput;
    }

    public async Task CreateChannelAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("Please select a workspace first");
            return;
        }

        _consoleOutput.WriteHeader("Create Channel");

        var name = _consoleInput.ReadRequiredString("Channel name");

        var request = new CreateChannelRequestDto
        {
            Name = name
        };

        var result = await _channelApiClient.CreateAsync(
            _userSession.CurrentWorkspace.Id,
            request);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Channel created successfully");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteChannel(result.Data!);
    }

    public async Task ListChannelsAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("Please select a workspace first");
            return;
        }

        _consoleOutput.WriteHeader("Channels");

        var result = await _channelApiClient.GetByWorkspaceIdAsync(
            _userSession.CurrentWorkspace.Id);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var channels = result.Data!;

        if (channels.Count == 0)
        {
            _consoleOutput.WriteInfo("No channels found");
            return;
        }

        var index = 1;

        foreach (var channel in channels)
        {
            _consoleOutput.WriteInfo(index.ToString());

            _consoleOutput.WriteChannel(channel);

            _consoleOutput.WriteSeparator();

            index++;
        }
    }

    public async Task SelectChannelAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("Please select a workspace first");
            return;
        }

        _consoleOutput.WriteHeader("Select Channel");

        var result = await _channelApiClient.GetByWorkspaceIdAsync(
            _userSession.CurrentWorkspace.Id);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var channels = result.Data!;

        if (channels.Count == 0)
        {
            _consoleOutput.WriteInfo("No channels found");
            return;
        }

        _consoleOutput.WriteChannelSelection(
            channels);

        var selection = _consoleInput.ReadInt(
            "Select channel",
            1,
            channels.Count);

        var channel = channels[selection - 1];

        var previousChannelId = _userSession.CurrentChannel?.Id;

        try
        {
            await _realtimeSessionManager.ChangeChannelAsync(
                previousChannelId,
                channel.Id);
        }
        catch (Exception ex)
        {
            _consoleOutput.WriteError($"Unable to change realtime channel: {ex.Message}");
            return;
        }

        _userSession.SelectChannel(channel);

        await _messageWorkflow.LoadMessagesAsync();

        _consoleOutput.WriteSuccess($"Channel '{channel.Name}' selected");
    }

    public async Task RenameChannelAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("Please select a workspace first");
            return;
        }

        _consoleOutput.WriteHeader("Rename Channel");

        var result = await _channelApiClient
            .GetByWorkspaceIdAsync(
                _userSession.CurrentWorkspace.Id);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var channels = result.Data!;

        if (channels.Count == 0)
        {
            _consoleOutput.WriteInfo("No channels found");
            return;
        }

        _consoleOutput.WriteChannelSelection(channels);

        var selection = _consoleInput.ReadInt(
            "Select channel",
            1,
            channels.Count);

        var channel = channels[selection - 1];

        var newName = _consoleInput.ReadRequiredString(
            "New channel name");

        var updateResult = await _channelApiClient.UpdateAsync(
            channel.Id,
            new UpdateChannelRequestDto
            {
                Name = newName
            });

        if (!updateResult.IsSuccess)
        {
            _consoleOutput.WriteError(updateResult.ErrorMessage!);
            return;
        }

        if (_userSession.CurrentChannel?.Id == channel.Id)
        {
            _userSession.SelectChannel(updateResult.Data!);
        }

        _consoleOutput.WriteSuccess("Channel renamed successfully");
    }

    public async Task DeleteChannelAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("Please select a workspace first");
            return;
        }

        _consoleOutput.WriteHeader("Delete Channel");

        var result = await _channelApiClient.GetByWorkspaceIdAsync(
            _userSession.CurrentWorkspace.Id);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var channels = result.Data!;

        if (channels.Count == 0)
        {
            _consoleOutput.WriteInfo("No channels found");
            return;
        }

        _consoleOutput.WriteChannelSelection(channels);

        var selection = _consoleInput.ReadInt(
            "Select channel",
            1,
            channels.Count);

        var channel = channels[selection - 1];

        var confirm = _consoleInput.ReadRequiredString(
            $"Type DELETE to remove '{channel.Name}'");

        if (!confirm.Equals(
                "DELETE",
                StringComparison.Ordinal))
        {
            _consoleOutput.WriteInfo("Operation cancelled");
            return;
        }

        var deleteResult = await _channelApiClient.DeleteAsync(
            channel.Id);

        if (!deleteResult.IsSuccess)
        {
            _consoleOutput.WriteError(deleteResult.ErrorMessage!);
            return;
        }

        if (_userSession.CurrentChannel?.Id == channel.Id)
        {
            await _realtimeSessionManager.LeaveChannelAsync(channel.Id);

            _userSession.ClearChannel();

            _messageCache.Clear();
        }

        _consoleOutput.WriteSuccess("Channel deleted successfully");
    }
}