namespace ChatApp.Domain.Entities;

/// <summary>
/// Represents a private conversation between two users
/// </summary>
public class PrivateConversation
{
    public Guid Id { get; set; }

    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User1 { get; set; } = null!;
    public User User2 { get; set; } = null!;

    /// <summary>
    /// Gets or sets the messages exchanged in this conversation
    /// </summary>
    public ICollection<PrivateMessage> Messages { get; set; }
        = new List<PrivateMessage>();
}