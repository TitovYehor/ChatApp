namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Identifies a member removed from a workspace
/// </summary>
public class WorkspaceMemberRemovedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string WorkspaceName { get; set; } = string.Empty;
}