using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Users.Requests;
using ChatApp.Contracts.Users.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileResponseDto>> GetProfile(
        Guid id)
    {
        var profile = await _userService.GetProfileAsync(id);

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponseDto>> UpdateProfile(
        UpdateUserProfileRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var profile = await _userService.UpdateProfileAsync(
            userId,
            request);

        return Ok(profile);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        await _userService.ChangePasswordAsync(
            userId,
            request);

        return NoContent();
    }
}