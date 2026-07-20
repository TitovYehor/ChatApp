namespace ChatApp.Contracts.Workspaces.Requests;

public class RemoveWorkspaceMemberRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
}