namespace ChatApp.Contracts.Realtime.Requests;

public class TypingStoppedRequest
{
    public Guid ChannelId { get; set; }
}