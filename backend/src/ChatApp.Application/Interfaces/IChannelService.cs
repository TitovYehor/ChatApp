using ChatApp.Contracts.Channels.Requests;
using ChatApp.Contracts.Channels.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines operations for managing workspace channels
/// </summary>
public interface IChannelService
{
    /// <summary>
    /// Creates a channel in the specified workspace
    /// </summary>
    Task<ChannelResponseDto> CreateAsync(
        Guid workspaceId,
        Guid userId,
        CreateChannelRequestDto request);

    /// <summary>
    /// Retrieves a channel by its identifier
    /// </summary>
    Task<ChannelResponseDto> GetByIdAsync(
        Guid channelId,
        Guid userId);

    /// <summary>
    /// Searches for channels in the specified workspace
    /// </summary>
    Task<IReadOnlyCollection<ChannelResponseDto>> SearchAsync(
        Guid workspaceId,
        Guid userId,
        ChannelSearchRequestDto request);

    /// <summary>
    /// Retrieves all channels in the specified workspace
    /// </summary>
    Task<IReadOnlyCollection<ChannelResponseDto>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Guid userId);

    /// <summary>
    /// Updates the specified channel
    /// </summary>
    Task<ChannelResponseDto> UpdateAsync(
        Guid channelId,
        Guid userId,
        UpdateChannelRequestDto request);

    /// <summary>
    /// Deletes the specified channel
    /// </summary>
    Task DeleteAsync(
        Guid channelId,
        Guid userId);
}