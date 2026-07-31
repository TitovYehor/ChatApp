namespace ChatApp.Contracts.Realtime.Requests;

public sealed class JoinChannelRequest
{
    public Guid ChannelId { get; set; }
}