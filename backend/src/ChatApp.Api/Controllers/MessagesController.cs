using ChatApp.Application.DTOs.Common;
using ChatApp.Application.DTOs.Messages;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

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
}