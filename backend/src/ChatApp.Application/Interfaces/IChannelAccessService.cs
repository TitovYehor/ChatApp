namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines operations for checking whether users can access channels
/// </summary>
public interface IChannelAccessService
{
    /// <summary>
    /// Determines whether the specified user can access the channel
    /// </summary>
    Task<bool> CanAccessChannelAsync(
        Guid userId,
        Guid channelId);
}