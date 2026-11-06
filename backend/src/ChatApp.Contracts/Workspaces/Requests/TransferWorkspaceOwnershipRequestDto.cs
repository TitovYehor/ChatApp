namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Identifies the workspace member who will become the new owner
/// </summary>
public class TransferWorkspaceOwnershipRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
}