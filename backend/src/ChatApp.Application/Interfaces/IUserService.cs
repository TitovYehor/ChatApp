using ChatApp.Contracts.Users.Requests;
using ChatApp.Contracts.Users.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines operations for retrieving and updating user profiles
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves the profile of the specified user
    /// </summary>
    Task<UserProfileResponseDto> GetProfileAsync(
        Guid userId);

    /// <summary>
    /// Updates the profile of the specified user
    /// </summary>
    Task<UserProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateUserProfileRequestDto request);

    /// <summary>
    /// Retrieves the public profile of the specified user
    /// </summary>
    Task<PublicUserProfileResponseDto> GetPublicProfileAsync(
        Guid userId);
}