namespace ChatApp.Contracts.Workspaces.Requests;

public class TransferWorkspaceOwnershipRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
}