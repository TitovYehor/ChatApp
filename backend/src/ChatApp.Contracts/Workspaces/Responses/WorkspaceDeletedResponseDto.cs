namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Identifies a workspace that has been deleted
/// </summary>
public class WorkspaceDeletedResponseDto
{
    public Guid WorkspaceId { get; set; }
}