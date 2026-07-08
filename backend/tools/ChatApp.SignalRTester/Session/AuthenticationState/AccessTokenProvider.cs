namespace ChatApp.SignalRTester.Session.AuthenticationState;

public class AccessTokenProvider : IAccessTokenProvider
{
    private readonly UserSession _userSession;

    public AccessTokenProvider(
        UserSession userSession)
    {
        _userSession = userSession;
    }

    public string? GetToken()
    {
        return _userSession.AccessToken;
    }
}