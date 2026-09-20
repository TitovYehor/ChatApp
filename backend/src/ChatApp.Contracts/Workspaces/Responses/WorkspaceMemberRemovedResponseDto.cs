namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceMemberRemovedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string WorkspaceName { get; set; } = string.Empty;
}