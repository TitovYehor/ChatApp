using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Infrastructure.Authentication;

/// <summary>
/// Provides hashing functionality for refresh tokens before they are stored
/// </summary>
public static class RefreshTokenHasher
{
    /// <summary>
    /// Computes a SHA-256 hash of the supplied refresh token
    /// </summary>
    public static string Hash(
        string refreshToken)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(
                refreshToken));

        return Convert.ToHexString(
            bytes);
    }
}