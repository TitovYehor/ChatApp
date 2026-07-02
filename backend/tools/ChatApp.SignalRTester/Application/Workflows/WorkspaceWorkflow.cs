using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.SignalRTester.Clients.Workspaces;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class WorkspaceWorkflow
{
    private readonly IWorkspaceApiClient _workspaceApiClient;

    private readonly UserSession _userSession;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    public WorkspaceWorkflow(
        IWorkspaceApiClient workspaceApiClient,
        UserSession userSession,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    { 
        _workspaceApiClient = workspaceApiClient;
        _userSession = userSession;
        _consoleInput = consoleInput;
        _consoleOutput = consoleOutput;
    }

    public async Task CreateWorkspaceAsync()
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

    public async Task ListWorkspacesAsync()
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

    public async Task SelectWorkspaceAsync()
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