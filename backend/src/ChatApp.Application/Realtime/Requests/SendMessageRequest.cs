namespace ChatApp.Application.Realtime.Requests;

public sealed class SendMessageRequest
{
    public Guid ChannelId { get; set; }

    public string Content { get; set; } = string.Empty;
}