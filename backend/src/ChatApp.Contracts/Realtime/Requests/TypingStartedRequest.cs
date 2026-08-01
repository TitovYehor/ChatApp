namespace ChatApp.Contracts.Realtime.Requests;

public class TypingStartedRequest
{
    public Guid ChannelId { get; set; }
}