using ChatApp.Application.DTOs.Channels;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class ChannelsController : ControllerBase
{
    private readonly IChannelService _channelService;
    private readonly ICurrentUserService _currentUserService;

    public ChannelsController(
        IChannelService channelService,
        ICurrentUserService currentUserService)
    {
        _channelService = channelService;
        _currentUserService = currentUserService;
    }

    [HttpPost("workspaces/{workspaceId:guid}/channels")]
    public async Task<ActionResult<ChannelResponseDto>> Create(
        Guid workspaceId,
        CreateChannelRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var channel = await _channelService.CreateAsync(
            workspaceId,
            userId,
            request);

        return CreatedAtAction(
            nameof(GetById),
            new { channelId = channel.Id },
            channel);
    }

    [HttpGet("channels/{channelId:guid}")]
    public async Task<ActionResult<ChannelResponseDto>> GetById(
        Guid channelId)
    {
        var userId = _currentUserService.GetUserId();

        var channel = await _channelService.GetByIdAsync(
            channelId,
            userId);

        return Ok(channel);
    }
}