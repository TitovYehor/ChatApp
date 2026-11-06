namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Contains the optional search query used to filter workspace members
/// </summary>
public class WorkspaceMemberSearchRequestDto
{
    public string? Query { get; set; }
}