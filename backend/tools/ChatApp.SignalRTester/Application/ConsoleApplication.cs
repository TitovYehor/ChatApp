using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.UI;
using ChatApp.SignalRTester.UI.Models;

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
        while (true)
        {
            var option = await _menu.ShowAsync();

            switch (option)
            {
                case MenuOption.Login:
                    Console.WriteLine();
                    Console.WriteLine("Login selected");
                    break;

                case MenuOption.Exit:
                    return;
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER...");
            Console.ReadLine();
        }
    }
}