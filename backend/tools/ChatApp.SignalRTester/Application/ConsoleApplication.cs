using ChatApp.SignalRTester.Application.Workflows;
using ChatApp.SignalRTester.UI;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.Application;

public class ConsoleApplication : IConsoleApplication
{
    private readonly IConsoleMenu _menu;

    private readonly AuthenticationWorkflow _authenticationWorkflow;

    private readonly WorkspaceWorkflow _workspaceWorkflow;

    private readonly ChannelWorkflow _channelWorkflow;

    private readonly SignalRWorkflow _signalRWorkflow;

    private readonly MessageWorkflow _messageWorkflow;

    public ConsoleApplication(
        IConsoleMenu menu,
        AuthenticationWorkflow authenticationWorkflow,
        WorkspaceWorkflow workspaceWorkflow,
        ChannelWorkflow channelWorkflow,
        SignalRWorkflow signalRWorkflow,
        MessageWorkflow messageWorkflow)
    {
        _menu = menu;
        _authenticationWorkflow = authenticationWorkflow;
        _workspaceWorkflow = workspaceWorkflow;
        _channelWorkflow = channelWorkflow;
        _signalRWorkflow = signalRWorkflow;
        _messageWorkflow = messageWorkflow;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            var option = await _menu.ShowAsync();

            if (option == null)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid selection");
                Console.WriteLine();

                continue;
            }

            switch (option.Value)
            {
                case MenuOption.Login:
                    await _authenticationWorkflow.LoginAsync();
                    break;

                case MenuOption.CreateWorkspace:
                    await _workspaceWorkflow.CreateWorkspaceAsync();
                    break;
                case MenuOption.ListWorkspaces:
                    await _workspaceWorkflow.ListWorkspacesAsync();
                    break;
                case MenuOption.SelectWorkspace:
                    await _workspaceWorkflow.SelectWorkspaceAsync();
                    break;

                case MenuOption.CreateChannel:
                    await _channelWorkflow.CreateChannelAsync();
                    break;

                case MenuOption.ListChannels:
                    await _channelWorkflow.ListChannelsAsync();
                    break;
                case MenuOption.SelectChannel:
                    await _channelWorkflow.SelectChannelAsync();
                    break;

                case MenuOption.LoadMessages:
                    await _messageWorkflow.LoadMessagesAsync();
                    break;
                case MenuOption.SendMessage:
                    await _messageWorkflow.SendMessageAsync();
                    break;
                case MenuOption.UpdateMessage:
                    await _messageWorkflow.UpdateMessageAsync();
                    break;
                case MenuOption.DeleteMessage:
                    await _messageWorkflow.DeleteMessageAsync();
                    break;

                case MenuOption.ConnectSignalR:
                    await _signalRWorkflow.ConnectAsync();
                    break;
                case MenuOption.DisconnectSignalR:
                    await _signalRWorkflow.DisconnectAsync();
                    break;

                case MenuOption.Logout:
                    await _authenticationWorkflow.LogoutAsync();
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