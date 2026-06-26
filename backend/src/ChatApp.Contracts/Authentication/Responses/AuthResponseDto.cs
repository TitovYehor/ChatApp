using ChatApp.Contracts.Users;

namespace ChatApp.Contracts.Authentication.Responses;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public AuthenticatedUserDto User { get; set; } = new();
}