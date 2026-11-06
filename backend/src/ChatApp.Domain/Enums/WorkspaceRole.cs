namespace ChatApp.Domain.Enums;

/// <summary>
/// Defines the roles a user can have within a workspace
/// </summary>
public enum WorkspaceRole
{
    /// <summary>
    /// The workspace owner with full control over the workspace
    /// </summary>
    Owner = 1,

    /// <summary>
    /// An administrator who can manage workspace resources and members
    /// </summary>
    Admin = 2,

    /// <summary>
    /// A regular workspace member
    /// </summary>
    Member = 3
}