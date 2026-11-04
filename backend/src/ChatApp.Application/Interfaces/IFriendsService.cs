using ChatApp.Contracts.Friends.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines operations for managing friendships and searching for users
/// </summary>
public interface IFriendsService
{
    /// <summary>
    /// Retrieves the friends of the specified user
    /// </summary>
    Task<IReadOnlyCollection<FriendResponseDto>> GetFriendsAsync(
        Guid currentUserId);

    /// <summary>
    /// Searches for users who can be added as friends
    /// </summary>
    Task<IReadOnlyCollection<FriendSearchResponseDto>> SearchUsersAsync(
        Guid currentUserId,
        string query);

    /// <summary>
    /// Adds the specified user as a friend
    /// </summary>
    Task AddFriendAsync(
        Guid currentUserId,
        Guid friendUserId);

    /// <summary>
    /// Removes the specified user from the current user's friends
    /// </summary>
    Task RemoveFriendAsync(
        Guid currentUserId,
        Guid friendUserId);
}