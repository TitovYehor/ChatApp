using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterRequestDto request)
    {
        var response = await _authService.RegisterAsync(request);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);

        return Ok(response);
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
}