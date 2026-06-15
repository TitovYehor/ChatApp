namespace ChatApp.Application.Realtime.Requests;

public sealed class LeaveChannelRequest
{
    public Guid ChannelId { get; set; }
}