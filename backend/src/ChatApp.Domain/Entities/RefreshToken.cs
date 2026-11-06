namespace ChatApp.Domain.Entities;

/// <summary>
/// Represents a refresh token associated with a user session
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the hash of the refresh token rather than its raw value
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the time when the token was revoked, or null if it has not been revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;

    /// <summary>
    /// Gets whether the token has not been revoked and has not expired
    /// </summary>
    public bool IsActive =>
        RevokedAt is null &&
        ExpiresAt > DateTime.UtcNow;
}