namespace ChatApp.Contracts.Channels.Responses;

/// <summary>
/// Identifies a deleted channel and the workspace it belonged to
/// </summary>
public class ChannelDeletedResponseDto
{
    public Guid ChannelId { get; set; }

    public Guid WorkspaceId { get; set; }
}