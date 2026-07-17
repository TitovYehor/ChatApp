using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceMemberResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public WorkspaceRoleDto Role { get; set; }

    public DateTime JoinedAt { get; set; }
}