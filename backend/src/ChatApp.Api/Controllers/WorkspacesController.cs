using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

/// <summary>
/// Provides endpoints for workspace management and membership operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    private readonly ICurrentUserService _currentUserService;

    public WorkspacesController(
        IWorkspaceService workspaceService,
        ICurrentUserService currentUserService)
    {
        _workspaceService = workspaceService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Creates a new workspace for the authenticated user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkspaceResponseDto>> Create(
        CreateWorkspaceRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var workspace = await _workspaceService.CreateAsync(
            userId,
            request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = workspace.Id },
            workspace);
    }

    /// <summary>
    /// Retrieves a workspace by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkspaceResponseDto>> GetById(
        Guid id)
    {
        var userId = _currentUserService.GetUserId();

        var workspace = await _workspaceService.GetByIdAsync(
            id,
            userId);

        return Ok(workspace);
    }

    /// <summary>
    /// Searches for workspaces available to the authenticated user
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyCollection<WorkspaceResponseDto>>> Search(
        [FromQuery] WorkspaceSearchRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var workspaces = await _workspaceService.SearchAsync(
            userId,
            request);

        return Ok(workspaces);
    }

    /// <summary>
    /// Retrieves all workspaces the authenticated user belongs to
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<WorkspaceResponseDto>>> GetAll()
    {
        var userId = _currentUserService.GetUserId();

        var workspaces = await _workspaceService
            .GetAllAsync(userId);

        return Ok(workspaces);
    }

    /// <summary>
    /// Updates workspace information
    /// </summary>
    [HttpPut("{workspaceId:guid}")]
    public async Task<ActionResult<WorkspaceResponseDto>> Update(
        Guid workspaceId,
        UpdateWorkspaceRequestDto request)
    {
        var workspace = await _workspaceService.UpdateAsync(
            workspaceId,
            _currentUserService.GetUserId(),
            request);

        return Ok(workspace);
    }

    /// <summary>
    /// Deletes a workspace
    /// </summary>
    [HttpDelete("{workspaceId:guid}")]
    public async Task<IActionResult> Delete(
        Guid workspaceId)
    {
        var userId = _currentUserService.GetUserId();

        await _workspaceService.DeleteAsync(
            workspaceId,
            userId);

        return NoContent();
    }

    /// <summary>
    /// Adds a user to a workspace
    /// </summary>
    [HttpPost("{workspaceId:guid}/members")]
    public async Task<ActionResult> AddMember(
        Guid workspaceId,
        AddWorkspaceMemberRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        await _workspaceService.AddMemberAsync(
            workspaceId,
            userId,
            request);

        return Ok();
    }

    /// <summary>
    /// Retrieves the members of a workspace
    /// </summary>
    [HttpGet("{workspaceId:guid}/members")]
    public async Task<ActionResult<IReadOnlyCollection<WorkspaceMemberResponseDto>>> GetMembers(
        Guid workspaceId)
    {
        var userId = _currentUserService.GetUserId();

        var members = await _workspaceService.GetMembersAsync(
            workspaceId,
            userId);

        return Ok(members);
    }

    /// <summary>
    /// Searches for members within a workspace
    /// </summary>
    [HttpGet("{workspaceId:guid}/members/search")]
    public async Task<ActionResult<IReadOnlyCollection<WorkspaceMemberResponseDto>>> SearchMembers(
        Guid workspaceId,
        [FromQuery] WorkspaceMemberSearchRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        var members = await _workspaceService.SearchMembersAsync(
            workspaceId,
            userId,
            request);

        return Ok(members);
    }

    /// <summary>
    /// Adds the authenticated user to a workspace
    /// </summary>
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join(
        Guid id)
    {
        var userId = _currentUserService.GetUserId();

        await _workspaceService.JoinAsync(
            id,
            userId);

        return NoContent();
    }

    /// <summary>
    /// Removes the authenticated user from a workspace
    /// </summary>
    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> Leave(
        Guid id)
    {
        var userId = _currentUserService.GetUserId();

        await _workspaceService.LeaveAsync(
            id,
            userId);

        return NoContent();
    }

    /// <summary>
    /// Removes a member from a workspace
    /// </summary>
    [HttpDelete("{workspaceId:guid}/members")]
    public async Task<IActionResult> RemoveMember(
        Guid workspaceId,
        RemoveWorkspaceMemberRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        await _workspaceService.RemoveMemberAsync(
            workspaceId,
            userId,
            request);

        return NoContent();
    }

    /// <summary>
    /// Changes a workspace member's role
    /// </summary>
    [HttpPut("{workspaceId:guid}/members/role")]
    public async Task<IActionResult> ChangeRole(
        Guid workspaceId,
        ChangeWorkspaceMemberRoleRequestDto request)
    {
        var userId = _currentUserService.GetUserId();

        await _workspaceService.ChangeMemberRoleAsync(
            workspaceId,
            userId,
            request);

        return NoContent();
    }

    /// <summary>
    /// Transfers workspace ownership to another member
    /// </summary>
    [HttpPut("{workspaceId:guid}/ownership")]
    public async Task<IActionResult> TransferOwnership(
        Guid workspaceId,
        TransferWorkspaceOwnershipRequestDto request)
    {
        var currentUserId = _currentUserService.GetUserId();

        await _workspaceService.TransferOwnershipAsync(
            workspaceId,
            currentUserId,
            request);

        return NoContent();
    }
}