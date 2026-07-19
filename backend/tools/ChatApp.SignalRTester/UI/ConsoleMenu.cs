using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.SignalR;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.UI;

public class ConsoleMenu : IConsoleMenu
{
    private readonly UserSession _session;

    private readonly ISignalRClient _signalRClient;

    public ConsoleMenu(
        UserSession session,
        ISignalRClient signalRClient)
    {
        _session = session;
        _signalRClient = signalRClient;
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

        Console.WriteLine($"SignalR: {(_signalRClient.IsConnected ? "Connected" : "Disconnected")}");

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
                Text = "Register",
                Option = MenuOption.Register,
                Visible = !_session.IsAuthenticated
            },
            new MenuItem
            {
                Number = 2,
                Text = "Login",
                Option = MenuOption.Login,
                Visible = !_session.IsAuthenticated
            },

            new MenuItem
            {
                Number = 3,
                Text = "Create workspace",
                Option = MenuOption.CreateWorkspace,
                Visible = _session.IsAuthenticated
            },
            new MenuItem
            {
                Number = 4,
                Text = "List workspaces",
                Option = MenuOption.ListWorkspaces,
                Visible = _session.IsAuthenticated
            },
            new MenuItem
            {
                Number = 5,
                Text = _session.CurrentWorkspace == null
                    ? "Select workspace"
                    : "Change workspace",
                Option = MenuOption.SelectWorkspace,
                Visible = _session.IsAuthenticated
            },
            new MenuItem
            {
                Number = 6,
                Text = "Add workspace member",
                Option = MenuOption.AddWorkspaceMember,
                Visible = _session.IsAuthenticated && 
                          _session.CurrentWorkspace != null
            },
            new MenuItem
            {
                Number = 7,
                Text = "List workspace members",
                Option = MenuOption.ListWorkspaceMembers,
                Visible =
                    _session.IsAuthenticated &&
                    _session.CurrentWorkspace != null
            },
            new MenuItem
            {
                Number = 8,
                Text = "Join workspace",
                Option = MenuOption.JoinWorkspace,
                Visible = _session.IsAuthenticated
            },

            new MenuItem
            {
                Number = 9,
                Text = "Create channel",
                Option = MenuOption.CreateChannel,
                Visible = _session.IsAuthenticated &&
                          _session.CurrentWorkspace != null
            },
            new MenuItem
            {
                Number = 10,
                Text = "List channels",
                Option = MenuOption.ListChannels,
                Visible = _session.IsAuthenticated &&
                          _session.CurrentWorkspace != null
            },
            new MenuItem
            {
                Number = 11,
                Text = _session.CurrentChannel == null
                    ? "Select channel"
                    : "Change channel",
                Option = MenuOption.SelectChannel,
                Visible = _session.IsAuthenticated &&
                          _session.CurrentWorkspace != null
            },

            new MenuItem
            {
                Number = 12,
                Text = "Load messages",
                Option = MenuOption.LoadMessages,
                Visible =
                    _session.IsAuthenticated &&
                    _session.CurrentChannel != null
            },
            new MenuItem
            {
                Number = 13,
                Text = "Send message",
                Option = MenuOption.SendMessage,
                Visible =
                    _session.IsAuthenticated &&
                    _session.CurrentChannel != null
            },
            new MenuItem
            {
                Number = 14,
                Text = "Update message",
                Option = MenuOption.UpdateMessage,
                Visible =
                    _session.IsAuthenticated &&
                    _session.CurrentChannel != null
            },
            new MenuItem
            {
                Number = 15,
                Text = "Delete message",
                Option = MenuOption.DeleteMessage,
                Visible =
                    _session.IsAuthenticated &&
                    _session.CurrentChannel != null
            },

            new MenuItem
            {
                Number = 16,
                Text = "Connect to SignalR",
                Option = MenuOption.ConnectSignalR,
                Visible = _session.IsAuthenticated &&
                          _session.CurrentWorkspace != null &&
                          _session.CurrentChannel != null &&
                          !_signalRClient.IsConnected
            },
            new MenuItem
            {
                Number = 17,
                Text = "Disconnect from SignalR",
                Option = MenuOption.DisconnectSignalR,
                Visible = _session.IsAuthenticated &&
                          _signalRClient.IsConnected
            },

            new MenuItem
            {
                Number = 18,
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