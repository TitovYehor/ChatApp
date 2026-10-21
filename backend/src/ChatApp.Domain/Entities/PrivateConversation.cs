namespace ChatApp.Domain.Entities;

public class PrivateConversation
{
    public Guid Id { get; set; }

    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User1 { get; set; } = null!;
    public User User2 { get; set; } = null!;

    public ICollection<PrivateMessage> Messages { get; set; }
        = new List<PrivateMessage>();
}