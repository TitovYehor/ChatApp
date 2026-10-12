namespace ChatApp.Domain.Entities;

public class Friendship
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid FriendUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public User FriendUser { get; set; } = null!;
}