using ChatApp.Contracts.Authentication.Responses;

namespace ChatApp.Application.Authentication;

public class AuthResult
{
    public AuthResponseDto Response { get; set; } = new();

    public string RefreshToken { get; set; } = string.Empty;
}