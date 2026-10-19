using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Friends.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendsController : ControllerBase
{
    private readonly IFriendsService _friendsService;
    private readonly ICurrentUserService _currentUserService;

    public FriendsController(
        IFriendsService friendsService,
        ICurrentUserService currentUserService)
    {
        _friendsService = friendsService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<FriendResponseDto>>>
        GetFriends()
    {
        var currentUserId = _currentUserService.GetUserId();

        var friends = await _friendsService.GetFriendsAsync(
            currentUserId);

        return Ok(friends);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyCollection<FriendSearchResponseDto>>>
        SearchUsers([FromQuery] string query = "")
    {
        var currentUserId = _currentUserService.GetUserId();

        var users = await _friendsService.SearchUsersAsync(
            currentUserId,
            query ?? string.Empty);

        return Ok(users);
    }

    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> AddFriend(
        Guid userId)
    {
        var currentUserId = _currentUserService.GetUserId();

        await _friendsService.AddFriendAsync(
            currentUserId,
            userId);

        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveFriend(
        Guid userId)
    {
        var currentUserId = _currentUserService.GetUserId();

        await _friendsService.RemoveFriendAsync(
            currentUserId,
            userId);

        return NoContent();
    }
}