using System;

namespace ChatApp.Contracts.Workspaces.Responses;

public class WorkspaceMemberRemovedResponseDto
{
    public Guid WorkspaceId { get; set; }

    public string WorkspaceName { get; set; } = string.Empty;
}