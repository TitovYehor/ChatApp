namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Identifies a workspace member to remove by username or email
/// </summary>
public class RemoveWorkspaceMemberRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
}