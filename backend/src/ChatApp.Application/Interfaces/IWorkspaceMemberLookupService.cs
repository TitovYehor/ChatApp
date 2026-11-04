using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines lookup operations for workspace members and their online presence
/// </summary>
public interface IWorkspaceMemberLookupService
{
    /// <summary>
    /// Retrieves the presence lookup information available to the specified user
    /// </summary>
    Task<PresenceLookupResponseDto> GetPresenceLookupAsync(
        Guid userId);

    /// <summary>
    /// Retrieves online users visible to the specified user
    /// </summary>
    Task<IReadOnlyCollection<OnlineUserResponseDto>> GetOnlineUsersAsync(
        Guid userId,
        IReadOnlyCollection<Guid> onlineUsers);
}