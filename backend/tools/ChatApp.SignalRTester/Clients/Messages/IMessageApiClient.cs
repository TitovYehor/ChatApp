using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.SignalRTester.Models;

namespace ChatApp.SignalRTester.Clients.Messages;

public interface IMessageApiClient
{
    Task<ApiResult<MessageResponseDto>> CreateAsync(
        Guid channelId,
        CreateMessageRequestDto request);

    Task<ApiResult<PagedResult<MessageResponseDto>>> GetByChannelAsync(
        Guid channelId,
        MessageQueryDto query);

    Task<ApiResult<MessageResponseDto>> GetByIdAsync(
        Guid messageId);

    Task<ApiResult<MessageResponseDto>> UpdateAsync(
        Guid messageId,
        UpdateMessageRequestDto request);

    Task<ApiResult<bool>> DeleteAsync(
        Guid messageId);
}