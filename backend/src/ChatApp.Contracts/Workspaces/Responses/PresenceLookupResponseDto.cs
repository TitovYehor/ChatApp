namespace ChatApp.Contracts.Workspaces.Responses;

/// <summary>
/// Contains the user information and recipient IDs needed to distribute presence updates
/// </summary>
public class PresenceLookupResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the IDs of users who should receive this user's presence updates
    /// </summary>
    public IReadOnlyCollection<Guid> RecipientUserIds { get; set; }
        = [];
}