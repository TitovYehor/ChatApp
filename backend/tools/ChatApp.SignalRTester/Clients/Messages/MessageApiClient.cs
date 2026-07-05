using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Models;
using ChatApp.SignalRTester.Session;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.Clients.Messages;

public class MessageApiClient: ApiClientBase, IMessageApiClient
{
    public MessageApiClient(
        IHttpClientFactory factory,
        IOptions<AppSettings> options,
        UserSession session)
        : base(factory.CreateClient(), session)
    {
        HttpClient.BaseAddress =
            new Uri(options.Value.ApiBaseUrl);
    }

    public Task<ApiResult<MessageResponseDto>> CreateAsync(
        Guid channelId,
        CreateMessageRequestDto request)
    {
        return PostAsync<CreateMessageRequestDto, MessageResponseDto>(
            $"api/channels/{channelId}/messages",
            request);
    }

    public Task<ApiResult<MessageResponseDto>> GetByIdAsync(
        Guid messageId)
    {
        return GetAsync<MessageResponseDto>(
            $"api/messages/{messageId}");
    }

    public Task<ApiResult<MessageResponseDto>> UpdateAsync(
        Guid messageId,
        UpdateMessageRequestDto request)
    {
        return PutAsync<UpdateMessageRequestDto, MessageResponseDto>(
            $"api/messages/{messageId}",
            request);
    }

    public Task<ApiResult<bool>> DeleteAsync(
        Guid messageId)
    {
        return DeleteRequestAsync($"api/messages/{messageId}");
    }

    public Task<ApiResult<PagedResult<MessageResponseDto>>> GetByChannelAsync(
        Guid channelId,
        MessageQueryDto query)
    {
        var url = AppendQueryString(
            $"api/channels/{channelId}/messages",
            new Dictionary<string, string?>
            {
                ["pageNumber"] = query.PageNumber.ToString(),
                ["pageSize"] = query.PageSize.ToString()
            });

        return GetAsync<PagedResult<MessageResponseDto>>(url);
    }
}