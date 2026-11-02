using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.Messages.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

/// <summary>
/// Provides endpoints for managing messages in workspace channels
/// </summary>
[ApiController]
[Route("api")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ICurrentUserService _currentUserService;

    public MessagesController(
        IMessageService messageService,
        ICurrentUserService currentUserService)
    {
        _messageService = messageService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Creates a message in a channel
    /// </summary>
    [HttpPost("channels/{channelId:guid}/messages")]
    public async Task<ActionResult<MessageResponseDto>> Create(
        Guid channelId,
        CreateMessageRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var message = await _messageService.CreateAsync(
            channelId,
            userId,
            request);

        return CreatedAtAction(
            nameof(GetById),
            new { messageId = message.Id },
            message);
    }

    /// <summary>
    /// Retrieves a message by ID
    /// </summary>
    [HttpGet("messages/{messageId:guid}")]
    public async Task<ActionResult<MessageResponseDto>> GetById(
        Guid messageId)
    {
        var userId = _currentUserService.GetUserId();

        var message = await _messageService.GetByIdAsync(
            messageId,
            userId);

        return Ok(message);
    }

    /// <summary>
    /// Retrieves a paginated list of messages from a channel
    /// </summary>
    [HttpGet("channels/{channelId:guid}/messages")]
    public async Task<ActionResult<PagedResult<MessageResponseDto>>> GetByChannelId(
        Guid channelId,
        [FromQuery] MessageQueryDto query)
    {
        var userId = _currentUserService.GetUserId();

        var result = await _messageService.GetByChannelIdAsync(
                channelId,
                userId,
                query);

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing message
    /// </summary>
    [HttpPut("messages/{messageId:guid}")]
    public async Task<ActionResult<MessageResponseDto>> Update(
        Guid messageId,
        UpdateMessageRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var message = await _messageService.UpdateAsync(
            messageId,
            userId,
            request);

        return Ok(message);
    }

    /// <summary>
    /// Deletes a message
    /// </summary>
    [HttpDelete("messages/{messageId:guid}")]
    public async Task<IActionResult> Delete(
        Guid messageId)
    {
        var userId = _currentUserService.GetUserId();

        await _messageService.DeleteAsync(
            messageId,
            userId);

        return NoContent();
    }
}