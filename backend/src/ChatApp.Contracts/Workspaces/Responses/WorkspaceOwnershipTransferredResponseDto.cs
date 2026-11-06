namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Describes a workspace ownership transfer, including the previous and new owners
/// </summary>
public class WorkspaceOwnershipTransferredResponseDto
{
    public Guid WorkspaceId { get; set; }

    public Guid PreviousOwnerUserId { get; set; }

    public string PreviousOwnerUsername { get; set; } = string.Empty;

    public Guid NewOwnerUserId { get; set; }

    public string NewOwnerUsername { get; set; } = string.Empty;
}