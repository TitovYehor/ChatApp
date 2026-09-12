using System;
using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceMemberAddedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public WorkspaceRoleDto CurrentUserRole { get; set; } = WorkspaceRoleDto.Member;

    public DateTime CreatedAt { get; set; }

    public string AddedByUsername { get; set; } = string.Empty;
}