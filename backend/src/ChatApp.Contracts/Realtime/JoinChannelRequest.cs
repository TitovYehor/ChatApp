namespace ChatApp.Contracts.Realtime;

public sealed class JoinChannelRequest
{
    public Guid ChannelId { get; set; }
}