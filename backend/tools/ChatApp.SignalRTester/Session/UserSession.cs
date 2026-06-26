using ChatApp.Contracts.Authentication.Responses;

namespace ChatApp.SignalRTester.Session;

public class UserSession
{
    public Guid UserId { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string? AccessToken { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(AccessToken);

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
    }
}