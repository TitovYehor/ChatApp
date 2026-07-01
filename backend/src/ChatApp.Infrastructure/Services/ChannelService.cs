using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappings;
using ChatApp.Contracts.Channels.Requests;
using ChatApp.Contracts.Channels.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ChatApp.Infrastructure.Services;

public class ChannelService : IChannelService
{
    private readonly AppDbContext _dbContext;

    private readonly IWorkspaceAuthorizationService _workspaceAuthorization;

    public ChannelService(
        AppDbContext dbContext,
        IWorkspaceAuthorizationService workspaceAuthorization)
    {
        this._dbContext = dbContext;
        this._workspaceAuthorization = workspaceAuthorization;
    }

    public async Task<ChannelResponseDto> CreateAsync(
        Guid workspaceId,
        Guid userId,
        CreateChannelRequestDto request)
    {
        await _workspaceAuthorization
            .EnsureCanCreateChannelAsync(
                workspaceId,
                userId);

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
        await _workspaceAuthorization
            .EnsureCanAccessWorkspaceAsync(
                workspaceId,
                userId);

        var channels = await _dbContext.Channels
            .AsNoTracking()
            .Where(x => x.WorkspaceId == workspaceId)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return channels
            .Select(x => x.ToDto())
            .ToList();
    }

    public async Task<ChannelResponseDto> UpdateAsync(
        Guid channelId,
        Guid userId,
        UpdateChannelRequestDto request)
    {
        var channel = await _workspaceAuthorization
            .GetManageableChannelAsync(
                channelId,
                userId);

        var newName = request.Name.Trim();

        if (channel.Name == newName)
        {
            return channel.ToDto();
        }

        channel.Name = newName;

        try
        {
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
                throw new ConflictException(
                    "Channel with this name already exists");
            }

            throw;
        }

        return channel.ToDto();
    }

    public async Task DeleteAsync(
        Guid channelId,
        Guid userId)
    {
        var channel = await _workspaceAuthorization
            .GetManageableChannelAsync(
                channelId,
                userId);

        _dbContext.Channels.Remove(channel);

        await _dbContext.SaveChangesAsync();
    }
}