namespace ChatApp.Contracts.Messages.Responses;

public class MessageDeletedResponseDto
{
    public Guid MessageId { get; set; }

    public Guid ChannelId { get; set; }
}