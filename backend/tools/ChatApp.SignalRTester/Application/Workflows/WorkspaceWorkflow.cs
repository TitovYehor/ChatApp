using ChatApp.Contracts.Workspaces.Enums;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.SignalRTester.Application.Services;
using ChatApp.SignalRTester.Clients.Workspaces;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class WorkspaceWorkflow
{
    private readonly IWorkspaceApiClient _workspaceApiClient;

    private readonly UserSession _userSession;

    private readonly RealtimeSessionManager _realtimeSessionManager;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    public WorkspaceWorkflow(
        IWorkspaceApiClient workspaceApiClient,
        UserSession userSession,
        RealtimeSessionManager realtimeSessionManager,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    { 
        _workspaceApiClient = workspaceApiClient;
        _userSession = userSession;
        _realtimeSessionManager = realtimeSessionManager;
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

        var previousChannelId = _userSession.CurrentChannel?.Id;

        if (previousChannelId.HasValue)
        {
            await _realtimeSessionManager.LeaveChannelAsync(
                previousChannelId.Value);
        }

        _userSession.SelectWorkspace(workspace);

        await RefreshWorkspaceRoleAsync();

        _consoleOutput.WriteInfo($"Role: {_userSession.CurrentWorkspaceRole}");

        _consoleOutput.WriteSuccess($"Workspace '{workspace.Name}' selected");
    }

    public async Task AddMemberAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("Please select a workspace first");
            return;
        }

        _consoleOutput.WriteHeader("Add Workspace Member");

        var usernameOrEmail = _consoleInput.ReadRequiredString(
            "Username or Email");

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = usernameOrEmail
        };

        var result = await _workspaceApiClient.AddMemberAsync(
            _userSession.CurrentWorkspace.Id,
            request);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Member added successfully");
    }

    public async Task ListMembersAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("Please select a workspace first");
            return;
        }

        _consoleOutput.WriteHeader("Workspace Members");

        var result = await _workspaceApiClient.GetMembersAsync(
            _userSession.CurrentWorkspace.Id);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteWorkspaceMembers(result.Data!);
    }

    public async Task JoinWorkspaceAsync()
    {
        _consoleOutput.WriteHeader("Join Workspace");

        var idText = _consoleInput.ReadRequiredString("Workspace Id");

        if (!Guid.TryParse(idText, out var workspaceId))
        {
            _consoleOutput.WriteError("Invalid workspace id");
            return;
        }

        var result = await _workspaceApiClient.JoinAsync(workspaceId);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Joined workspace successfully");
    }

    public async Task LeaveWorkspaceAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("No workspace selected");
            return;
        }

        if (_userSession.CurrentWorkspaceRole == WorkspaceRoleDto.Owner)
        {
            _consoleOutput.WriteError("Transfer workspace ownership before leaving");
            return;
        }

        var confirmed = _consoleInput.ReadConfirmation(
            $"Leave workspace '{_userSession.CurrentWorkspace.Name}'?");

        if (!confirmed)
        {
            _consoleOutput.WriteInfo("Operation cancelled");
            return;
        }

        var workspaceId = _userSession.CurrentWorkspace.Id;

        var result = await _workspaceApiClient.LeaveAsync(workspaceId);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var currentChannel = _userSession.CurrentChannel?.Id;

        _userSession.ClearWorkspace();

        if (currentChannel.HasValue)
        {
            await _realtimeSessionManager.LeaveChannelAsync(
                currentChannel.Value);
        }

        _consoleOutput.WriteSuccess("Left workspace successfully");
    }

    public async Task RemoveMemberAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("No workspace selected");
            return;
        }

        _consoleOutput.WriteHeader("Remove Workspace Member");

        var usernameOrEmail = _consoleInput.ReadRequiredString(
            "Username or Email");

        var confirmed = _consoleInput.ReadConfirmation(
            $"Remove user '{usernameOrEmail}'?");

        if (!confirmed)
        {
            _consoleOutput.WriteInfo("Operation cancelled");
            return;
        }

        var result = await _workspaceApiClient.RemoveMemberAsync(
            _userSession.CurrentWorkspace.Id,
            new RemoveWorkspaceMemberRequestDto
            {
                UsernameOrEmail = usernameOrEmail
            });

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Member removed successfully");
    }

    public async Task ChangeMemberRoleAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("No workspace selected");
            return;
        }

        _consoleOutput.WriteHeader("Change member role");

        var user = _consoleInput.ReadRequiredString(
            "Username or email");

        Console.WriteLine();
        Console.WriteLine("1. Member");
        Console.WriteLine("2. Admin");

        var option = _consoleInput.ReadInt(
            "Role",
            1,
            2);

        var role = option == 2
            ? WorkspaceRoleDto.Admin
            : WorkspaceRoleDto.Member;

        var request = new ChangeWorkspaceMemberRoleRequestDto
        {
            UsernameOrEmail = user,
            Role = role
        };

        var result = await _workspaceApiClient
            .ChangeMemberRoleAsync(
                _userSession.CurrentWorkspace.Id,
                request);

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Member role updated");
    }

    public async Task TransferOwnershipAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            _consoleOutput.WriteError("No workspace selected");
            return;
        }

        _consoleOutput.WriteHeader("Transfer Workspace Ownership");

        var usernameOrEmail = _consoleInput.ReadRequiredString(
            "Username or email");

        var confirm = _consoleInput.ReadRequiredString(
            "Type TRANSFER to confirm");

        if (!confirm.Equals(
                "TRANSFER",
                StringComparison.Ordinal))
        {
            _consoleOutput.WriteInfo("Operation cancelled");
            return;
        }

        var result = await _workspaceApiClient.TransferOwnershipAsync(
            _userSession.CurrentWorkspace.Id,
            new TransferWorkspaceOwnershipRequestDto
            {
                UsernameOrEmail = usernameOrEmail
            });

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        _consoleOutput.WriteSuccess("Workspace ownership transferred");
    }

    private async Task RefreshWorkspaceRoleAsync()
    {
        if (_userSession.CurrentWorkspace == null)
        {
            return;
        }

        var result = await _workspaceApiClient
            .GetMembersAsync(_userSession.CurrentWorkspace.Id);

        if (!result.IsSuccess)
        {
            return;
        }

        var me = result.Data!
            .FirstOrDefault(x => x.UserId == _userSession.UserId);

        if (me != null)
        {
            _userSession.SetWorkspaceRole(me.Role);
        }
    }
}