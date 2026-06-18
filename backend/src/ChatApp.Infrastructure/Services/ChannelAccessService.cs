using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public sealed class ChannelAccessService
    : IChannelAccessService
{
    private readonly AppDbContext _dbContext;

    public ChannelAccessService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

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