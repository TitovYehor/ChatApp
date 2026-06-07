namespace ChatApp.Application.DTOs.Channels;

public class ChannelResponseDto
{
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Type { get; set; }

    public DateTime CreatedAt { get; set; }
}