namespace ChatApp.Contracts.Workspaces.Responses;

public class PresenceLookupResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public IReadOnlyCollection<Guid> RecipientUserIds { get; set; }
        = [];
}