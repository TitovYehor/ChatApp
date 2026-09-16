namespace ChatApp.Contracts.Channels.Responses;

public class ChannelDeletedResponseDto
{
    public Guid ChannelId { get; set; }
    public Guid WorkspaceId { get; set; }
}