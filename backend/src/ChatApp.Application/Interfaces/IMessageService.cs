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
}