using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.PrivateConversations.Responses;

namespace ChatApp.Application.Interfaces;

public interface IPrivateConversationService
{
    Task<PrivateConversationResponseDto> GetOrCreateAsync(
        Guid currentUserId,
        Guid otherUserId);

    Task<IReadOnlyCollection<PrivateConversationResponseDto>>
        GetAllAsync(Guid currentUserId);

    Task<PagedResult<PrivateMessageResponseDto>> GetMessagesAsync(
        Guid currentUserId,
        Guid conversationId,
        MessageQueryDto query);

    Task<PrivateMessageResponseDto> SendMessageAsync(
        Guid currentUserId,
        Guid conversationId,
        CreateMessageRequestDto request);

    Task<bool> IsParticipantAsync(
        Guid userId,
        Guid conversationId);
}