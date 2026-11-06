using ChatApp.Contracts.Workspaces.Enums;

namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Represents workspace information returned to API clients
/// </summary>
public class WorkspaceResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the current user's role in the workspace, or null when no role is included
    /// </summary>
    public WorkspaceRoleDto? CurrentUserRole { get; set; }
}