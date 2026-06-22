using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.UI;

namespace ChatApp.SignalRTester.Application;

public class ConsoleApplication : IConsoleApplication
{
    private readonly IConsoleMenu _menu;

    private readonly IAuthenticationApiClient _authenticationApiClient;


    public ConsoleApplication(
        IConsoleMenu menu,
        IAuthenticationApiClient authenticationApiClient)
    {
        _menu = menu;
        _authenticationApiClient = authenticationApiClient;
    }

    public async Task RunAsync()
    {
        await _menu.RunAsync();
    }
}