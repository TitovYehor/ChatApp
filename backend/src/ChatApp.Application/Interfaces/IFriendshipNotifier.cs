namespace ChatApp.Application.Interfaces;

public interface IFriendshipNotifier
{
    Task FriendshipChangedAsync(
        IReadOnlyCollection<Guid> userIds);
}