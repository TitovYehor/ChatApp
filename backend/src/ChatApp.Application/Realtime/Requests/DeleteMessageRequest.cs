namespace ChatApp.Application.Realtime.Requests;

public sealed class DeleteMessageRequest
{
    public Guid MessageId { get; set; }
}