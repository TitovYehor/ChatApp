using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceMemberRoleChangedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public Guid UserId { get; set; }

    public WorkspaceRoleDto Role { get; set; }
}