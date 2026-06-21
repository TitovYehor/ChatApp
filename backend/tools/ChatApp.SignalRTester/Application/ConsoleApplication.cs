using ChatApp.SignalRTester.UI;

namespace ChatApp.SignalRTester.Application;

public class ConsoleApplication : IConsoleApplication
{
    private readonly IConsoleMenu _menu;

    public ConsoleApplication(
        IConsoleMenu menu)
    {
        _menu = menu;
    }

    public async Task RunAsync()
    {
        await _menu.RunAsync();
    }
}