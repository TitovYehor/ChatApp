using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.PrivateConversations.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines operations for managing private conversations and their messages
/// </summary>
public interface IPrivateConversationService
{
    /// <summary>
    /// Retrieves an existing conversation with the specified user or creates one
    /// </summary>
    Task<PrivateConversationResponseDto> GetOrCreateAsync(
        Guid currentUserId,
        Guid otherUserId);

    /// <summary>
    /// Retrieves all private conversations of the specified user
    /// </summary>
    Task<IReadOnlyCollection<PrivateConversationResponseDto>>
        GetAllAsync(Guid currentUserId);

    /// <summary>
    /// Retrieves a paginated list of messages in the specified conversation
    /// </summary>
    Task<PagedResult<PrivateMessageResponseDto>> GetMessagesAsync(
        Guid currentUserId,
        Guid conversationId,
        MessageQueryDto query);

    /// <summary>
    /// Sends a message in the specified private conversation
    /// </summary>
    Task<PrivateMessageResponseDto> SendMessageAsync(
        Guid currentUserId,
        Guid conversationId,
        CreateMessageRequestDto request);

    /// <summary>
    /// Updates a message in the specified private conversation
    /// </summary>
    Task<PrivateMessageResponseDto> UpdateMessageAsync(
        Guid currentUserId,
        Guid conversationId,
        Guid messageId,
        UpdateMessageRequestDto request);

    /// <summary>
    /// Deletes a message from the specified private conversation
    /// </summary>
    Task DeleteMessageAsync(
        Guid currentUserId,
        Guid conversationId,
        Guid messageId);

    /// <summary>
    /// Determines whether the specified user participates in the conversation
    /// </summary>
    Task<bool> IsParticipantAsync(
        Guid userId,
        Guid conversationId);
}