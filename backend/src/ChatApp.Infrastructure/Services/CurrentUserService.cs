using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChatApp.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var userIdClaim =
            _httpContextAccessor
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