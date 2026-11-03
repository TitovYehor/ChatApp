using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.Messages.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines operations for managing messages in channels
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Creates a message in the specified channel
    /// </summary>
    Task<MessageResponseDto> CreateAsync(
        Guid channelId,
        Guid userId,
        CreateMessageRequestDto request);

    /// <summary>
    /// Retrieves a message by its identifier
    /// </summary>
    Task<MessageResponseDto> GetByIdAsync(
        Guid messageId,
        Guid userId);

    /// <summary>
    /// Retrieves a paginated list of messages from the specified channel
    /// </summary>
    Task<PagedResult<MessageResponseDto>> GetByChannelIdAsync(
        Guid channelId,
        Guid userId,
        MessageQueryDto query);

    /// <summary>
    /// Updates the specified message
    /// </summary>
    Task<MessageResponseDto> UpdateAsync(
        Guid messageId,
        Guid userId,
        UpdateMessageRequestDto request);

    /// <summary>
    /// Deletes the specified message
    /// </summary>
    Task DeleteAsync(
        Guid messageId,
        Guid userId);
}