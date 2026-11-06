namespace ChatApp.Contracts.Realtime.Requests;

/// <summary>
/// Identifies the channel in which a user has stopped typing
/// </summary>
public class TypingStoppedRequest
{
    public Guid ChannelId { get; set; }
}