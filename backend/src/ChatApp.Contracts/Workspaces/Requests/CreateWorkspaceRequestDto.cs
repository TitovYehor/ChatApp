namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Contains the information required to create a workspace
/// </summary>
public class CreateWorkspaceRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}