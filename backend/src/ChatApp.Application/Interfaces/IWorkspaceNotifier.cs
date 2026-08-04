namespace ChatApp.Application.Interfaces;

public interface IWorkspaceNotifier
{
    Task WorkspaceDeletedAsync(
        Guid workspaceId,
        IReadOnlyCollection<Guid> memberIds);
}