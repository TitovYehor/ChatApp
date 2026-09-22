using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IAuthService authService,
        IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.Response);
    }

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

    private void SetRefreshTokenCookie(
        string refreshToken)
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