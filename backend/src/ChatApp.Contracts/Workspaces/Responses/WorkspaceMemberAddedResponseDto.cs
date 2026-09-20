using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceMemberAddedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public WorkspaceRoleDto Role { get; set; } = WorkspaceRoleDto.Member;

    public DateTime CreatedAt { get; set; }

    public DateTime JoinedAt { get; set; }

    public string AddedByUsername { get; set; } = string.Empty;
}