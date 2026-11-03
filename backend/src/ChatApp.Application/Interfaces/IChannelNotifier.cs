using ChatApp.Contracts.Channels.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines realtime notifications for channel changes
/// </summary>
public interface IChannelNotifier
{
    /// <summary>
    /// Notifies workspace members that a channel has been created
    /// </summary>
    Task ChannelCreatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        ChannelResponseDto response);

    /// <summary>
    /// Notifies workspace members that a channel has been updated
    /// </summary>
    Task ChannelUpdatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        ChannelResponseDto response);

    /// <summary>
    /// Notifies workspace members that a channel has been deleted
    /// </summary>
    Task ChannelDeletedAsync(
        IReadOnlyCollection<Guid> memberIds,
        ChannelDeletedResponseDto response);
}