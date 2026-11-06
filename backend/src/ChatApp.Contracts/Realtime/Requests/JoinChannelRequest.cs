namespace ChatApp.Contracts.Realtime.Requests;

/// <summary>
/// Identifies the channel a client requests to join
/// </summary>
public sealed class JoinChannelRequest
{
    public Guid ChannelId { get; set; }
}