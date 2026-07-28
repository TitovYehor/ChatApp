using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class WorkspaceMemberLookupService
    : IWorkspaceMemberLookupService
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
}