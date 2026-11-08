using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Services;

/// <summary>
/// Resolves SignalR user identifiers from authenticated connection claims
/// </summary>
public sealed class UserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Gets the user identifier from the connection's name identifier claim
    /// </summary>
    public string? GetUserId(
        HubConnectionContext connection)
    {
        return connection.User?
            .FindFirst(
                ClaimTypes.NameIdentifier)?
            .Value;
    }
}