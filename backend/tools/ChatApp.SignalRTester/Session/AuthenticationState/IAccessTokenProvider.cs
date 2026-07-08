namespace ChatApp.SignalRTester.Session.AuthenticationState;

public interface IAccessTokenProvider
{
    string? GetToken();
}