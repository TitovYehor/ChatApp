using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Identifies a workspace member and their newly assigned role
/// </summary>
public class WorkspaceMemberRoleChangedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public Guid UserId { get; set; }

    public WorkspaceRoleDto Role { get; set; }
}