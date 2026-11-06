using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Requests;

/// <summary>
/// Identifies a workspace member and the role to assign to them
/// </summary>
public class ChangeWorkspaceMemberRoleRequestDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;

    public WorkspaceRoleDto Role { get; set; }
}