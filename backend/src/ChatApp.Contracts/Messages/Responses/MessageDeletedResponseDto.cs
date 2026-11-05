namespace ChatApp.Contracts.Messages.Responses;

/// <summary>
/// Identifies a deleted message and the channel it belonged to
/// </summary>
public class MessageDeletedResponseDto
{
    public Guid MessageId { get; set; }

    public Guid ChannelId { get; set; }
}