namespace ChatApp.Domain.Entities;

/// <summary>
/// Represents a notification delivered to a user
/// </summary>
public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RecipientUserId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who triggered the notification, if applicable
    /// </summary>
    public Guid? ActorUserId { get; set; }

    /// <summary>
    /// Gets or sets the notification type
    /// </summary>
    public string Type { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the related workspace ID, if the notification is associated with a workspace
    /// </summary>
    public Guid? WorkspaceId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the related entity, if applicable
    /// </summary>
    public Guid? EntityId { get; set; }
}