namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Contains the updated name and description of a workspace
/// </summary>
public class UpdateWorkspaceRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}