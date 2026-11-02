using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Users.Requests;
using ChatApp.Contracts.Users.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

/// <summary>
/// Provides endpoints for retrieving and updating user profiles
/// </summary>
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

    /// <summary>
    /// Retrieves a user's profile by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileResponseDto>> GetProfile(
        Guid id)
    {
        var profile = await _userService.GetProfileAsync(id);

        return Ok(profile);
    }

    /// <summary>
    /// Updates the authenticated user's profile
    /// </summary>
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

    /// <summary>
    /// Retrieves the publicly visible information of a user
    /// </summary>
    [HttpGet("{id:guid}/public")]
    public async Task<ActionResult<PublicUserProfileResponseDto>> GetPublicProfile(
        Guid id)
    {
        var profile = await _userService.GetPublicProfileAsync(id);

        return Ok(profile);
    }
}