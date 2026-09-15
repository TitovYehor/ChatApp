using ChatApp.Contracts.Channels.Responses;

namespace ChatApp.Application.Interfaces;

public interface IChannelNotifier
{
    Task ChannelCreatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        ChannelResponseDto response);
}