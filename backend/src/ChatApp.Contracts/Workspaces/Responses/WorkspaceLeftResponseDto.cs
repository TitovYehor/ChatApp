namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceLeftResponseDto
{
    public Guid WorkspaceId { get; set; }

    public Guid UserId { get; set; }
}