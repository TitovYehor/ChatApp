namespace ChatApp.Contracts.Workspaces.Requests;

public class CreateWorkspaceRequestDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}