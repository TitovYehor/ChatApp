using ChatApp.Application.DTOs.Channels;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappings;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ChatApp.Infrastructure.Services;

public class ChannelService : IChannelService
{
    private readonly AppDbContext _dbContext;

    public ChannelService(
        AppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<ChannelResponseDto> CreateAsync(
        Guid workspaceId,
        Guid userId,
        CreateChannelRequestDto request)
    {
        var isMember = await _dbContext.WorkspaceMembers
            .AnyAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId);

        if (!isMember)
        {
            throw new ForbiddenException("User is not a member of the workspace");
        }

        var channel = new Channel
        {
            WorkspaceId = workspaceId,
            Name = request.Name,
            Type = ChannelType.Text
        };

        try
        {
            _dbContext.Channels.Add(channel);

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException postgresException
                   && postgresException.SqlState ==
                      PostgresErrorCodes.UniqueViolation)
        {
            if (postgresException.ConstraintName ==
                DatabaseConstraintNames.UniqueChannelNamePerWorkspace)
            {
                throw new ConflictException("Channel with this name already exists");
            }

            throw;
        }

        return channel.ToDto();
    }

    public async Task<ChannelResponseDto> GetByIdAsync(
        Guid channelId,
        Guid userId)
    {
        var channel = await _dbContext.Channels
            .AsNoTracking()
            .Where(c => c.Id == channelId)
            .Where(c =>
                c.Workspace.Members.Any(
                    m => m.UserId == userId))
            .FirstOrDefaultAsync();

        if (channel == null)
        {
            throw new NotFoundException("Channel not found");
        }

        return channel.ToDto();
    }

    public async Task<IReadOnlyCollection<ChannelResponseDto>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Guid userId)
    {
        var isMember = await _dbContext.WorkspaceMembers
            .AnyAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId);

        if (!isMember)
        {
            throw new ForbiddenException("User is not a member of this workspace");
        }

        var channels = await _dbContext.Channels
            .AsNoTracking()
            .Where(x => x.WorkspaceId == workspaceId)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return channels
            .Select(x => x.ToDto())
            .ToList();
    }
}