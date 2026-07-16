namespace ChatApp.Contracts.Workspaces.Requests;

public class AddWorkspaceMemberRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
}