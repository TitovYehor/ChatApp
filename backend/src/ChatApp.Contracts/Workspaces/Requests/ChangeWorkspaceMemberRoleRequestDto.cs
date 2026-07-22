using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Requests;

public class ChangeWorkspaceMemberRoleRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;

    public WorkspaceRoleDto Role { get; set; }
}