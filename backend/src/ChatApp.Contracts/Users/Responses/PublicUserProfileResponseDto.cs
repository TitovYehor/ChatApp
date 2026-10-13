namespace ChatApp.Contracts.Users.Responses;

public class PublicUserProfileResponseDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}