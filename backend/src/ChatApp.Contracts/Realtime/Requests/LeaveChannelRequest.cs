namespace ChatApp.Contracts.Realtime.Requests;

/// <summary>
/// Identifies the channel a client requests to leave
/// </summary>
public sealed class LeaveChannelRequest
{
    public Guid ChannelId { get; set; }
}