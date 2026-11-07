using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

/// <summary>
/// Checks whether users have access to channels through workspace membership
/// </summary>
public sealed class ChannelAccessService : IChannelAccessService
{
    private readonly AppDbContext _dbContext;

    public ChannelAccessService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<bool> CanAccessChannelAsync(
        Guid userId,
        Guid channelId)
    {
        return await _dbContext.Channels
            .AnyAsync(channel =>
                channel.Id == channelId
                && channel.Workspace.Members
                    .Any(member =>
                        member.UserId == userId));
    }
}