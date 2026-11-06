namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Contains the optional search query used to filter workspaces
/// </summary>
public class WorkspaceSearchRequestDto
{
    public string? Query { get; set; }
}