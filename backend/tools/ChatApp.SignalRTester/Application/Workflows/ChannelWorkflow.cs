using ChatApp.Contracts.Channels.Requests;
using ChatApp.SignalRTester.Clients.Channels;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class ChannelWorkflow
{
    private readonly IChannelApiClient _channelApiClient;

    private readonly UserSession _userSession;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    public ChannelWorkflow(
        IChannelApiClient channelApiClient,
        UserSession userSession,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    {
        _channelApiClient = channelApiClient;
        _userSession = userSession;
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

        _userSession.SelectChannel(channel);

        _consoleOutput.WriteSuccess($"Channel '{channel.Name}' selected");
    }
}