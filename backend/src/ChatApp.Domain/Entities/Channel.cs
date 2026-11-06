using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

/// <summary>
/// Represents a communication channel within a workspace
/// </summary>
public class Channel
{
    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ChannelType Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the workspace that contains this channel
    /// </summary>
    public Workspace Workspace { get; set; } = null!;

    /// <summary>
    /// Gets or sets the messages posted in this channel
    /// </summary>
    public ICollection<Message> Messages { get; set; }
        = new List<Message>();
}