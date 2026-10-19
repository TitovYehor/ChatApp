using ChatApp.Contracts.Friends.Responses;

namespace ChatApp.Application.Interfaces;

public interface IFriendsService
{
    Task<IReadOnlyCollection<FriendResponseDto>> GetFriendsAsync(
        Guid currentUserId);

    Task<IReadOnlyCollection<FriendSearchResponseDto>> SearchUsersAsync(
        Guid currentUserId,
        string query);

    Task AddFriendAsync(
        Guid currentUserId,
        Guid friendUserId);

    Task RemoveFriendAsync(
        Guid currentUserId,
        Guid friendUserId);
}