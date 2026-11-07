using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChatApp.Infrastructure.Services;

/// <summary>
/// Retrieves the authenticated user's identifier from the current HTTP context
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public Guid GetUserId()
    {
        var userIdClaim = _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new UserNotAuthenticatedException();
        }

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UserNotAuthenticatedException();
        }

        return userId;
    }
}