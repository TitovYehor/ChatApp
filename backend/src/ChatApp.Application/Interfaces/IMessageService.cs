using ChatApp.Application.DTOs.Common;
using ChatApp.Application.DTOs.Messages;

namespace ChatApp.Application.Interfaces;

public interface IMessageService
{
    Task<MessageResponseDto> CreateAsync(
        Guid channelId,
        Guid userId,
        CreateMessageRequestDto request);

    Task<MessageResponseDto> GetByIdAsync(
        Guid messageId,
        Guid userId);

    Task<PagedResult<MessageResponseDto>> GetByChannelIdAsync(
        Guid channelId,
        Guid userId,
        MessageQueryDto query);

    Task<MessageResponseDto> UpdateAsync(
        Guid messageId,
        Guid userId,
        UpdateMessageRequestDto request);
}