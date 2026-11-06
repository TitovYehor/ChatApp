namespace ChatApp.Contracts.Realtime.Requests;

/// <summary>
/// Identifies the channel in which a user has started typing
/// </summary>
public class TypingStartedRequest
{
    public Guid ChannelId { get; set; }
}