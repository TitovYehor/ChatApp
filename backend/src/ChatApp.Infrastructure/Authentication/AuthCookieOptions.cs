namespace ChatApp.Infrastructure.Authentication;

public static class AuthCookieOptions
{
    public const string RefreshTokenCookieName =
        "chatapp_refresh_token";

    public const string Path = "/api/auth";
}