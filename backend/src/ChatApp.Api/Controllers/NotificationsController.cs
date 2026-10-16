using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Notifications.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(
        INotificationService notificationService,
        ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationResponseDto>>> GetLatest(
        [FromQuery] int limit = 50)
    {
        if (limit < 1 || limit > 100)
        {
            return BadRequest("Limit must be between 1 and 100.");
        }

        var userId = _currentUserService.GetUserId();

        var notifications = await _notificationService.GetLatestAsync(
            userId,
            limit);

        return Ok(notifications);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id)
    {
        var userId = _currentUserService.GetUserId();

        var success = await _notificationService.MarkAsReadAsync(
            id,
            userId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = _currentUserService.GetUserId();

        var updatedCount = await _notificationService.MarkAllAsReadAsync(
            userId);

        return Ok(new { updatedCount });
    }
}