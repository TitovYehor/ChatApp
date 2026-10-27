using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.PrivateConversations.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/conversations")]
[Authorize]
public class PrivateConversationsController : ControllerBase
{
    private readonly IPrivateConversationService _conversationService;
    private readonly ICurrentUserService _currentUserService;

    public PrivateConversationsController(
        IPrivateConversationService conversationService,
        ICurrentUserService currentUserService)
    {
        _conversationService = conversationService;
        _currentUserService = currentUserService;
    }

    [HttpPost("{otherUserId:guid}")]
    public async Task<ActionResult<PrivateConversationResponseDto>> GetOrCreate(
        Guid otherUserId)
    {
        var currentUserId = _currentUserService.GetUserId();

        var conversation = await _conversationService.GetOrCreateAsync(
            currentUserId,
            otherUserId);

        return Ok(conversation);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PrivateConversationResponseDto>>> GetAll()
    {
        var currentUserId = _currentUserService.GetUserId();

        var conversations = await _conversationService.GetAllAsync(
            currentUserId);

        return Ok(conversations);
    }

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<PagedResult<PrivateMessageResponseDto>>> GetMessages(
        Guid conversationId,
        [FromQuery] MessageQueryDto query)
    {
        var currentUserId = _currentUserService.GetUserId();

        var messages = await _conversationService.GetMessagesAsync(
            currentUserId,
            conversationId,
            query);

        return Ok(messages);
    }

    [HttpPost("{conversationId:guid}/messages")]
    public async Task<ActionResult<PrivateMessageResponseDto>> SendMessage(
        Guid conversationId,
        CreateMessageRequestDto request)
    {
        var currentUserId = _currentUserService.GetUserId();

        var message = await _conversationService.SendMessageAsync(
            currentUserId,
            conversationId,
            request);

        return Ok(message);
    }

    [HttpPut("{conversationId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult<PrivateMessageResponseDto>> UpdateMessage(
        Guid conversationId,
        Guid messageId,
        UpdateMessageRequestDto request)
    {
        var currentUserId = _currentUserService.GetUserId();

        var message = await _conversationService.UpdateMessageAsync(
            currentUserId,
            conversationId,
            messageId,
            request);

        return Ok(message);
    }

    [HttpDelete("{conversationId:guid}/messages/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(
        Guid conversationId,
        Guid messageId)
    {
        var currentUserId = _currentUserService.GetUserId();

        await _conversationService.DeleteMessageAsync(
            currentUserId,
            conversationId,
            messageId);

        return NoContent();
    }
}