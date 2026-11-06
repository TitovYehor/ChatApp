namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Identifies the user who left a workspace
/// </summary>
public class WorkspaceLeftResponseDto
{
    public Guid WorkspaceId { get; set; }

    public Guid UserId { get; set; }
}