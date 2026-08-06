namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceUpdatedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}