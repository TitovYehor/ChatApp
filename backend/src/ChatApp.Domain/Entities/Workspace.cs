namespace ChatApp.Domain.Entities;

/// <summary>
/// Represents a workspace containing members and communication channels
/// </summary>
public class Workspace
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the members belonging to this workspace
    /// </summary>
    public ICollection<WorkspaceMember> Members { get; set; }
        = new List<WorkspaceMember>();

    /// <summary>
    /// Gets or sets the channels created within this workspace
    /// </summary>
    public ICollection<Channel> Channels { get; set; }
        = new List<Channel>();
}