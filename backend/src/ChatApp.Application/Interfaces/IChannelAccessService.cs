namespace ChatApp.Application.Interfaces;

public interface IChannelAccessService
{
    Task<bool> CanAccessChannelAsync(
        Guid userId,
        Guid channelId);
}