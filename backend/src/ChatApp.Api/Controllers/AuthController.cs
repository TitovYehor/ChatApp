using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.Contracts.Users.Requests;
using ChatApp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatApp.Api.Controllers;

/// <summary>
/// Provides endpoints for user registration, authentication,
/// token refresh, logout, and password management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService,
        IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _jwtSettings = jwtOptions.Value;
    }

    /// <summary>
    /// Registers a new user and issues authentication tokens
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    /// <summary>
    /// Authenticates a user and issues authentication tokens
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    /// <summary>
    /// Issues a new access token using the refresh token stored in the request cookie
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh()
    {
        var refreshToken = Request.Cookies[
            AuthCookieOptions.RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized();
        }

        var result = await _authService.RefreshAsync(refreshToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    /// <summary>
    /// Revokes the refresh token and removes the authentication cookie
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[
            AuthCookieOptions.RefreshTokenCookieName];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Changes the authenticated user's password and issues updated authentication tokens
    /// </summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<AuthResponseDto>> ChangePassword(
        ChangePasswordRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var result = await _authService.ChangePasswordAsync(
            userId,
            request);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    /// <summary>
    /// Returns basic information about the currently authenticated user
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            message = "You are authenticated",
            username = User.Identity?.Name
        });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            AuthCookieOptions.RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(
                    _jwtSettings.RefreshTokenExpirationDays),
                Path = AuthCookieOptions.Path
            });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            AuthCookieOptions.RefreshTokenCookieName,
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = AuthCookieOptions.Path
            });
    }
}