using ChatApp.Contracts.Authentication.Responses;
using ChatApp.Contracts.Channels.Responses;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.SignalRTester.Session;

public class UserSession
{
    public Guid UserId { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string? AccessToken { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);

    public WorkspaceResponseDto? CurrentWorkspace { get; private set; }

    public ChannelResponseDto? CurrentChannel { get; private set; }

    public void SignIn(
        AuthResponseDto response)
    {
        UserId = response.User.Id;
        Username = response.User.Username;
        Email = response.User.Email;
        AccessToken = response.AccessToken;
    }

    public void SignOut()
    {
        UserId = Guid.Empty;
        Username = string.Empty;
        Email = string.Empty;
        AccessToken = null;
        CurrentWorkspace = null;
        CurrentChannel = null;
    }

    public void SelectWorkspace(
        WorkspaceResponseDto workspace)
    {
        CurrentWorkspace = workspace;
        CurrentChannel = null;
    }

    public void ClearWorkspace()
    {
        CurrentWorkspace = null;
        CurrentChannel = null;
    }

    public Guid? SelectChannel(
        ChannelResponseDto channel)
    {
        var previous = CurrentChannel?.Id;

        CurrentChannel = channel;

        return previous;
    }
}