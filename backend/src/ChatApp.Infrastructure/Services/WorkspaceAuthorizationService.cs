using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class WorkspaceAuthorizationService: IWorkspaceAuthorizationService
{
    private readonly AppDbContext _dbContext;

    public WorkspaceAuthorizationService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureCanAccessWorkspaceAsync(
        Guid workspaceId,
        Guid userId)
    {
        var isMember = await _dbContext.WorkspaceMembers
            .AnyAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId);

        if (!isMember)
        {
            throw new ForbiddenException(
                "User is not a member of this workspace");
        }
    }

    public async Task EnsureCanCreateChannelAsync(
        Guid workspaceId,
        Guid userId)
    {
        var role = await _dbContext.WorkspaceMembers
            .Where(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId)
            .Select(x => (WorkspaceRole?)x.Role)
            .FirstOrDefaultAsync();

        if (role == null)
        {
            throw new ForbiddenException(
                "User is not a member of this workspace");
        }

        if (role != WorkspaceRole.Owner &&
            role != WorkspaceRole.Admin)
        {
            throw new ForbiddenException(
                "Only workspace administrators can create channels");
        }
    }

    public async Task EnsureCanManageChannelAsync(
        Guid channelId,
        Guid userId)
    {
        var role = await _dbContext.Channels
            .Where(c => c.Id == channelId)
            .SelectMany(c => c.Workspace.Members)
            .Where(m => m.UserId == userId)
            .Select(m => (WorkspaceRole?)m.Role)
            .FirstOrDefaultAsync();

        if (role == null)
        {
            throw new ForbiddenException(
                "User does not have access to this channel");
        }

        if (role != WorkspaceRole.Owner &&
            role != WorkspaceRole.Admin)
        {
            throw new ForbiddenException(
                "Only workspace administrators can manage channels");
        }
    }

    public async Task<Channel> GetManageableChannelAsync(
        Guid channelId,
        Guid userId)
    {
        var channel = await _dbContext.Channels
            .Include(c => c.Workspace)
            .ThenInclude(w => w.Members)
            .FirstOrDefaultAsync(c => c.Id == channelId);

        if (channel == null)
        {
            throw new NotFoundException("Channel not found");
        }

        var member = channel.Workspace.Members
            .FirstOrDefault(m => m.UserId == userId);

        if (member == null)
        {
            throw new ForbiddenException(
                "User does not have access to this channel");
        }

        if (member.Role != WorkspaceRole.Owner &&
            member.Role != WorkspaceRole.Admin)
        {
            throw new ForbiddenException(
                "Only workspace administrators can manage channels");
        }

        return channel;
    }
}