using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines realtime notifications for workspace and membership changes
/// </summary>
public interface IWorkspaceNotifier
{
    /// <summary>
    /// Notifies workspace members that the workspace has been deleted
    /// </summary>
    Task WorkspaceDeletedAsync(
        Guid workspaceId,
        IReadOnlyCollection<Guid> memberIds);

    /// <summary>
    /// Notifies workspace members that the workspace has been updated
    /// </summary>
    Task WorkspaceUpdatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceUpdatedResponseDto response);

    /// <summary>
    /// Notifies workspace members that a new member has joined or been added
    /// </summary>
    Task WorkspaceMemberAddedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceMemberAddedResponseDto response);

    /// <summary>
    /// Notifies workspace members that a member has been removed
    /// </summary>
    Task WorkspaceMemberRemovedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceMemberRemovedResponseDto response);

    /// <summary>
    /// Notifies workspace members that a member's role has changed
    /// </summary>
    Task WorkspaceMemberRoleChangedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceMemberRoleChangedResponseDto response);

    /// <summary>
    /// Notifies workspace members that ownership has been transferred
    /// </summary>
    Task WorkspaceOwnershipTransferredAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceOwnershipTransferredResponseDto response);

    /// <summary>
    /// Notifies workspace members that a user has left the workspace
    /// </summary>
    Task WorkspaceLeftAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceLeftResponseDto response);
}