namespace ChatApp.Contracts.Notifications.Responses;

public class NotificationResponseDto
{
    public Guid Id { get; set; }

    public Guid? ActorUserId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public Guid? WorkspaceId { get; set; }

    public Guid? EntityId { get; set; }
}