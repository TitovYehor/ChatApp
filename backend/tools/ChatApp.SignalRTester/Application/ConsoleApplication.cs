using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.Clients.Workspaces;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Models;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application;

public class ConsoleApplication : IConsoleApplication
{
    private readonly IConsoleMenu _menu;

    private readonly IAuthenticationApiClient _authenticationApiClient;

    private readonly IWorkspaceApiClient _workspaceApiClient;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    private readonly UserSession _userSession;

    public ConsoleApplication(
        IConsoleMenu menu,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput,
        IAuthenticationApiClient authenticationApiClient,
        IWorkspaceApiClient workspaceApiClient,
        UserSession userSession)
    {
        _menu = menu;
        _consoleInput = consoleInput;
        _consoleOutput = consoleOutput;
        _authenticationApiClient = authenticationApiClient;
        _workspaceApiClient = workspaceApiClient;
        _userSession = userSession;
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
                    await LoginAsync();
                    break;

                case MenuOption.CreateWorkspace:
                    await CreateWorkspaceAsync();
                    break;

                case MenuOption.ListWorkspaces:
                    await ListWorkspacesAsync();
                    break;

                case MenuOption.SelectWorkspace:
                    await SelectWorkspaceAsync();
                    break;

                case MenuOption.Logout:
                    Logout();
                    break;

                case MenuOption.Exit:
                    return;
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER...");
            Console.ReadLine();
        }
    }

    private async Task LoginAsync()
    {
        _consoleOutput.WriteHeader("Login");

        var email = _consoleInput.ReadRequiredString("Email");

        var password = _consoleInput.ReadRequiredString("Password");

        var request = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        var result = await _authenticationApiClient.LoginAsync(request);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var response = result.Data!;

        _userSession.SignIn(response);

        _consoleOutput.WriteSuccess("Login successful");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo($"Welcome {_userSession.Username}!");
        _consoleOutput.WriteInfo($"Email: {_userSession.Email}");
    }

    private void Logout()
    {
        _userSession.SignOut();

        _consoleOutput.WriteSeparator();
        _consoleOutput.WriteSuccess("Logged out successfully");
    }

    private async Task CreateWorkspaceAsync()
    {
        _consoleOutput.WriteHeader("Create Workspace");

        var name = _consoleInput.ReadRequiredString("Name");

        var description = _consoleInput.ReadRequiredString("Description");

        var request = new CreateWorkspaceRequestDto
        {
            Name = name,
            Description = description
        };

        var result = await _workspaceApiClient.CreateAsync(request);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var workspace = result.Data!;

        _consoleOutput.WriteSuccess("Workspace created successfully");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteWorkspace(workspace);
    }

    private async Task ListWorkspacesAsync()
    {
        _consoleOutput.WriteHeader("Workspaces");

        var result = await _workspaceApiClient.GetAllAsync();

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var workspaces = result.Data!;

        if (workspaces.Count == 0)
        {
            _consoleOutput.WriteInfo("No workspaces found");
            return;
        }

        var index = 1;

        foreach (var workspace in workspaces)
        {
            _consoleOutput.WriteInfo($"{index}");
            _consoleOutput.WriteWorkspace(workspace);
            
            _consoleOutput.WriteSeparator();

            index++;
        }
    }

    private async Task SelectWorkspaceAsync()
    {
        _consoleOutput.WriteHeader("Select Workspace");

        var result = await _workspaceApiClient.GetAllAsync();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var workspaces = result.Data!;

        if (workspaces.Count == 0)
        {
            _consoleOutput.WriteInfo("No workspaces found");
            return;
        }

        _consoleOutput.WriteWorkspaceSelection(workspaces);

        var selection = _consoleInput.ReadInt(
            "Select workspace",
            1,
            workspaces.Count);

        var workspace = workspaces[selection - 1];

        _userSession.SelectWorkspace(workspace);

        _consoleOutput.WriteSuccess($"Workspace '{workspace.Name}' selected");
    }
}