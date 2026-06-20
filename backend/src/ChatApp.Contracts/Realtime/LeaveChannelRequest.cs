namespace ChatApp.Contracts.Realtime;

public sealed class LeaveChannelRequest
{
    public Guid ChannelId { get; set; }
}