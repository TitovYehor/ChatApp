using ChatApp.Contracts.Authentication.Requests;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.UI;

public class ConsoleMenu : IConsoleMenu
{
    private readonly IAuthenticationApiClient _authenticationApiClient;

    private readonly UserSession _session;

    public ConsoleMenu(
        IAuthenticationApiClient authenticationApiClient,
        UserSession session)
    {
        _authenticationApiClient = authenticationApiClient;
        _session = session;
    }

    public async Task<MenuOption> ShowAsync()
    {
        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("    ChatApp SignalR Tester    ");
        Console.WriteLine("==============================");
        Console.WriteLine();

        if (_session.IsAuthenticated)
        {
            Console.WriteLine($"Signed in as {_session.Username}");
        }
        else
        {
            Console.WriteLine("Not signed in");
        }

        Console.WriteLine();

        if (!_session.IsAuthenticated)
        {
            Console.WriteLine("1. Login");
        }
        else
        {
            Console.WriteLine("2. Logout");
        }

        Console.WriteLine("0. Exit");

        Console.Write("Select option: ");

        var input = Console.ReadLine();

        if (!_session.IsAuthenticated)
        {
            return input switch
            {
                "1" => MenuOption.Login,
                _ => MenuOption.Exit
            };
        }

        return input switch
        {
            "2" => MenuOption.Logout,
            _ => MenuOption.Exit
        };
    }
}