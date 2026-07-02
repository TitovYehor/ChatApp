using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.SignalRTester.Application.Workflows;
using ChatApp.SignalRTester.UI;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.Application;

public class ConsoleApplication : IConsoleApplication
{
    private readonly IConsoleMenu _menu;

    private readonly AuthenticationWorkflow _authenticationWorkflow;

    private readonly WorkspaceWorkflow _workspaceWorkflow;

    public ConsoleApplication(
        IConsoleMenu menu,
        AuthenticationWorkflow authenticationWorkflow,
        WorkspaceWorkflow workspaceWorkflow)
    {
        _menu = menu;
        _authenticationWorkflow = authenticationWorkflow;
        _workspaceWorkflow = workspaceWorkflow;
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

                case MenuOption.Logout:
                    _authenticationWorkflow.Logout();
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