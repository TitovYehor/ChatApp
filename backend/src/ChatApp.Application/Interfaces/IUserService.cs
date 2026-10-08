using ChatApp.Contracts.Users.Requests;
using ChatApp.Contracts.Users.Responses;

namespace ChatApp.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileResponseDto> GetProfileAsync(
        Guid userId);

    Task<UserProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateUserProfileRequestDto request);

    Task ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequestDto request);
}