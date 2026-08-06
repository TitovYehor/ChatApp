using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Services;

public class RealtimeSessionManager
{
    private readonly ISignalRClient _signalRClient;

    private readonly RealtimeSession _realtimeSession;

    private readonly UserSession _userSession;

    private readonly MessageCache _messageCache;

    private readonly OnlineUsersCache _onlineUsersCache;

    private readonly IConsoleOutput _consoleOutput;

    public RealtimeSessionManager(
        ISignalRClient signalRClient,
        RealtimeSession realtimeSession,
        UserSession userSession,
        MessageCache messageCache,
        OnlineUsersCache onlineUsersCache,
        IConsoleOutput consoleOutput)
    {
        _signalRClient = signalRClient;
        _realtimeSession = realtimeSession;
        _userSession = userSession;
        _messageCache = messageCache;
        _onlineUsersCache = onlineUsersCache;
        _consoleOutput = consoleOutput;

        _signalRClient.Connected += OnConnected;

        _signalRClient.Disconnected += OnDisconnected;

        _signalRClient.Reconnected += OnReconnected;

        _signalRClient.WorkspaceDeleted += OnWorkspaceDeleted;

        _signalRClient.WorkspaceUpdated += OnWorkspaceUpdated;
    }

    public bool IsConnected => _signalRClient.IsConnected;

    public async Task ConnectAsync()
    {
        if (_signalRClient.IsConnected)
        {
            return;
        }

        await _signalRClient.ConnectAsync();

        if (_userSession.CurrentChannel != null)
        {
            await _signalRClient.JoinChannelAsync(
                _userSession.CurrentChannel.Id);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_signalRClient.IsConnected)
        {
            await _signalRClient.DisconnectAsync();
        }

        _onlineUsersCache.Clear();

        _realtimeSession.MarkDisconnected();
    }

    public async Task ChangeChannelAsync(
        Guid? previousChannelId,
        Guid newChannelId)
    {
        await ConnectAsync();

        if (previousChannelId.HasValue)
        {
            await _signalRClient.LeaveChannelAsync(
                previousChannelId.Value);
        }

        await _signalRClient.JoinChannelAsync(
            newChannelId);
    }

    public async Task LeaveChannelAsync(
        Guid channelId)
    {
        if (!_signalRClient.IsConnected)
        {
            return;
        }

        await _signalRClient.LeaveChannelAsync(channelId);
    }

    private void OnConnected()
    {
        _realtimeSession.MarkConnected();
    }

    private void OnDisconnected()
    {
        _realtimeSession.MarkDisconnected();
    }

    private async Task OnReconnected()
    {
        try
        {
            _realtimeSession.MarkConnected();

            if (_userSession.CurrentChannel == null)
            {
                return;
            }

            await _signalRClient.JoinChannelAsync(
                _userSession.CurrentChannel.Id);
        }
        catch (Exception ex)
        {
            _consoleOutput.WriteError($"Unable to restore realtime channel: {ex.Message}");
        }
    }

    private void OnWorkspaceDeleted(
        WorkspaceDeletedResponseDto response)
    {
        if (_userSession.CurrentWorkspace?.Id != response.WorkspaceId)
        {
            return;
        }

        _userSession.ClearWorkspace();

        _messageCache.Clear();

        _onlineUsersCache.Clear();

        _consoleOutput.WriteInfo("Workspace has been deleted");
    }

    private void OnWorkspaceUpdated(
        WorkspaceUpdatedResponseDto response)
    {
        if (_userSession.CurrentWorkspace?.Id != response.WorkspaceId)
        {
            return;
        }

        _userSession.UpdateWorkspace(
            response.Name,
            response.Description);

        _consoleOutput.WriteInfo($"Workspace renamed to '{response.Name}'");
    }
}