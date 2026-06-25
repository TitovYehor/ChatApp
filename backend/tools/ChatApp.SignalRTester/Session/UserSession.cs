namespace ChatApp.SignalRTester.Session;

public class UserSession
{
    public string? Token { get; private set; }

    public string? Username { get; private set; }

    public string? Email { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(Token);

    public void SignIn(
        string token,
        string username,
        string email)
    {
        Token = token;
        Username = username;
        Email = email;
    }

    public void SignOut()
    {
        Token = null;
        Username = null;
        Email = null;
    }
}