namespace ChatApp.Application.Realtime.Requests;

public sealed class UpdateMessageRequest
{
    public Guid MessageId { get; set; }

    public string Content { get; set; } = string.Empty;
}