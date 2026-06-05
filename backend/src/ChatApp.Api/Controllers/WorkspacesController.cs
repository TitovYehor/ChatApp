using ChatApp.Application.DTOs.Workspaces;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<WorkspaceResponseDto>>> GetAll()
    {
        var userId = _currentUserService.GetUserId();

        var workspaces = await _workspaceService
            .GetAllAsync(userId);

        return Ok(workspaces);
    }
}