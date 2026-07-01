namespace ChatApp.Application.Interfaces;

public interface IWorkspaceAuthorizationService
{
    Task EnsureCanAccessWorkspaceAsync(
        Guid workspaceId,
        Guid userId);

    Task EnsureCanCreateChannelAsync(
        Guid workspaceId,
        Guid userId);

    Task EnsureCanManageChannelAsync(
        Guid channelId,
        Guid userId);
}