using ChatApp.Contracts.Authentication.Requests;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.UI;

public class ConsoleMenu : IConsoleMenu
{
    private readonly IAuthenticationApiClient
        _authenticationApiClient;

    public ConsoleMenu(
        IAuthenticationApiClient authenticationApiClient)
    {
        _authenticationApiClient = authenticationApiClient;
    }

    public async Task<MenuOption> ShowAsync()
    {
        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("    ChatApp SignalR Tester    ");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.WriteLine("1. Login");
        Console.WriteLine("0. Exit");
        Console.WriteLine();

        Console.Write("Select option: ");

        var input = Console.ReadLine();

        return input switch
        {
            "1" => MenuOption.Login,
            _ => MenuOption.Exit
        };
    }
}