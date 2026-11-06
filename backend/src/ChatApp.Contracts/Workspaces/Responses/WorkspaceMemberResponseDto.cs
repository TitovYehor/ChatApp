using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Represents a member of a workspace, including their role and join date
/// </summary>
public class WorkspaceMemberResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public WorkspaceRoleDto Role { get; set; }

    public DateTime JoinedAt { get; set; }
}