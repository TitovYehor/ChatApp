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

    public async Task<MenuOption?> ShowAsync()
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

        var menuItems = BuildMenu();

        foreach (var item in menuItems.Where(x => x.Visible))
        {
            Console.WriteLine($"{item.Number}. {item.Text}");
        }

        Console.Write("Select option: ");

        var input = Console.ReadLine();

        if (!int.TryParse(input, out var selectedNumber))
        {
            return null;
        }

        var selectedItem = menuItems
            .FirstOrDefault(x =>
                x.Visible &&
                x.Number == selectedNumber);

        return selectedItem?.Option;
    }

    private IReadOnlyList<MenuItem> BuildMenu()
    {
        return
        [
            new MenuItem
        {
            Number = 1,
            Text = "Login",
            Option = MenuOption.Login,
            Visible = !_session.IsAuthenticated
        },

        new MenuItem
        {
            Number = 2,
            Text = "Logout",
            Option = MenuOption.Logout,
            Visible = _session.IsAuthenticated
        },

        new MenuItem
        {
            Number = 0,
            Text = "Exit",
            Option = MenuOption.Exit
        }
        ];
    }
}