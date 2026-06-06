using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

public class Channel
{
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ChannelType Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Workspace Workspace { get; set; } = null!;
}