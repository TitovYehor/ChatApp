using ChatApp.Contracts.Channels.Requests;
using ChatApp.Contracts.Channels.Responses;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

/// <summary>
/// Provides endpoints for creating, retrieving, updating, and deleting workspace channels
/// </summary>
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

    /// <summary>
    /// Creates a channel in a workspace
    /// </summary>
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

    /// <summary>
    /// Retrieves a channel by ID
    /// </summary>
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

    /// <summary>
    /// Searches for channels within a workspace
    /// </summary>
    [HttpGet("workspaces/{workspaceId:guid}/channels/search")]
    public async Task<ActionResult<IReadOnlyCollection<ChannelResponseDto>>> Search(
        Guid workspaceId,
        [FromQuery] ChannelSearchRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var channels = await _channelService.SearchAsync(
            workspaceId,
            userId,
            request);

        return Ok(channels);
    }

    /// <summary>
    /// Retrieves all channels belonging to a workspace
    /// </summary>
    [HttpGet("workspaces/{workspaceId:guid}/channels")]
    public async Task<ActionResult<IReadOnlyCollection<ChannelResponseDto>>> GetByWorkspaceId(
        Guid workspaceId)
    {
        var userId = _currentUserService.GetUserId();

        var channels = await _channelService.GetByWorkspaceIdAsync(
            workspaceId,
            userId);

        return Ok(channels);
    }

    /// <summary>
    /// Updates channel information
    /// </summary>
    [HttpPut("channels/{channelId:guid}")]
    public async Task<ActionResult<ChannelResponseDto>> Update(
        Guid channelId,
        UpdateChannelRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var channel = await _channelService.UpdateAsync(
            channelId,
            userId,
            request);

        return Ok(channel);
    }

    /// <summary>
    /// Deletes a channel
    /// </summary>
    [HttpDelete("channels/{channelId:guid}")]
    public async Task<IActionResult> Delete(
        Guid channelId)
    {
        var userId = _currentUserService.GetUserId();

        await _channelService.DeleteAsync(
            channelId,
            userId);

        return NoContent();
    }
}