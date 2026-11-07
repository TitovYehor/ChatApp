namespace ChatApp.Infrastructure.Authentication;

/// <summary>
/// Defines cookie settings used for refresh-token authentication
/// </summary>
public static class AuthCookieOptions
{
    /// <summary>
    /// Name of the cookie that stores the refresh token
    /// </summary>
    public const string RefreshTokenCookieName =
        "chatapp_refresh_token";

    /// <summary>
    /// API path to which the refresh-token cookie is scoped
    /// </summary>
    public const string Path = "/api/auth";
}