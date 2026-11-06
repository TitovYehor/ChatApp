namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Identifies a user to add to a workspace by username or email
/// </summary>
public class AddWorkspaceMemberRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
}