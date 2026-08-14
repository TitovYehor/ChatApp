using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class WorkspaceMemberLookupService : IWorkspaceMemberLookupService
{
    private readonly AppDbContext _dbContext;

    public WorkspaceMemberLookupService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PresenceLookupResponseDto> GetPresenceLookupAsync(
        Guid userId)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var recipients = await _dbContext.WorkspaceMembers
            .Where(x => x.UserId != userId)
            .Where(x =>
                _dbContext.WorkspaceMembers.Any(y =>
                    y.WorkspaceId == x.WorkspaceId &&
                    y.UserId == userId))
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync();

        return new PresenceLookupResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            RecipientUserIds = recipients
        };
    }

    public async Task<IReadOnlyCollection<OnlineUserResponseDto>> GetOnlineUsersAsync(
        Guid userId,
        IReadOnlyCollection<Guid> onlineUsers)
    {
        return await _dbContext.WorkspaceMembers
            .Where(m => m.UserId != userId)
            .Where(m =>
                onlineUsers.Contains(m.UserId))
            .Where(m =>
                _dbContext.WorkspaceMembers.Any(x =>
                    x.UserId == userId &&
                    x.WorkspaceId == m.WorkspaceId))
            .Select(m => new OnlineUserResponseDto
            {
                UserId = m.User.Id,
                Username = m.User.Username
            })
            .Distinct()
            .ToListAsync();
    }
}