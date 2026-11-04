using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines authorization checks for workspace and channel operations
/// </summary>
public interface IWorkspaceAuthorizationService
{
    /// <summary>
    /// Ensures that the specified user can access the workspace
    /// </summary>
    Task EnsureCanAccessWorkspaceAsync(
        Guid workspaceId,
        Guid userId);

    /// <summary>
    /// Ensures that the specified user can create channels in the workspace
    /// </summary>
    Task EnsureCanCreateChannelAsync(
        Guid workspaceId,
        Guid userId);

    /// <summary>
    /// Retrieves a channel that the specified user is allowed to manage
    /// </summary>
    Task<Channel> GetManageableChannelAsync(
        Guid channelId,
        Guid userId);
}