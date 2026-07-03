using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.UI;

public class ConsoleMenu : IConsoleMenu
{
    private readonly UserSession _session;

    public ConsoleMenu(
        UserSession session)
    {
        _session = session;
    }

    public async Task<MenuOption?> ShowAsync()
    {
        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("    ChatApp SignalR Tester    ");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.WriteLine($"User: {_session.Username}");

        Console.WriteLine($"Workspace: {(_session.CurrentWorkspace?.Name ?? "-")}");

        Console.WriteLine($"Channel: {(_session.CurrentChannel?.Name ?? "-")}");

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
            Number = 1,
            Text = "Create workspace",
            Option = MenuOption.CreateWorkspace,
            Visible = _session.IsAuthenticated
        },

        new MenuItem
        {
            Number = 2,
            Text = "List workspaces",
            Option = MenuOption.ListWorkspaces,
            Visible = _session.IsAuthenticated
        },

        new MenuItem
        {
            Number = 3,
            Text = _session.CurrentWorkspace == null
                ? "Select workspace"
                : "Change workspace",
            Option = MenuOption.SelectWorkspace,
            Visible = _session.IsAuthenticated
        },

        new MenuItem
        {
            Number = 4,
            Text = "Create channel",
            Option = MenuOption.CreateChannel,
            Visible = _session.IsAuthenticated &&
                      _session.CurrentWorkspace != null
        },

        new MenuItem
        {
            Number = 5,
            Text = "List channels",
            Option = MenuOption.ListChannels,
            Visible = _session.IsAuthenticated &&
                      _session.CurrentWorkspace != null
        },

        new MenuItem
        {
            Number = 6,
            Text = _session.CurrentChannel == null
                ? "Select channel"
                : "Change channel",
            Option = MenuOption.SelectChannel,
            Visible = _session.IsAuthenticated &&
                      _session.CurrentWorkspace != null
        },

        new MenuItem
        {
            Number = 9,
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