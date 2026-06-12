using ChatApp.Application.DTOs.Common;
using ChatApp.Application.DTOs.Messages;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappings;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly AppDbContext _dbContext;

    public MessageService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MessageResponseDto> CreateAsync(
        Guid channelId, 
        Guid userId, 
        CreateMessageRequestDto request)
    {
        var channel = await _dbContext.Channels
            .Include(x => x.Workspace)
            .ThenInclude(x => x.Members)
            .FirstOrDefaultAsync(x =>
                x.Id == channelId);

        if (channel == null)
        {
            throw new NotFoundException("Channel not found");
        }

        var isMember = channel.Workspace
            .Members
            .Any(x => x.UserId == userId);

        if (!isMember)
        {
            throw new ForbiddenException("User is not a member of this workspace");
        }

        var message = new Message
        {
            ChannelId = channelId,
            UserId = userId,
            Content = request.Content.Trim()
        };

        _dbContext.Messages.Add(message);

        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(message)
            .Reference(x => x.User)
            .LoadAsync();

        return message.ToDto();
    }

    public async Task<MessageResponseDto> GetByIdAsync(
        Guid messageId,
        Guid userId)
    {
        var message = await _dbContext.Messages
            .AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Id == messageId &&
                x.Channel.Workspace.Members.Any(
                    m => m.UserId == userId));

        if (message == null)
        {
            throw new NotFoundException("Message not found");
        }

        return message.ToDto();
    }

    public async Task<PagedResult<MessageResponseDto>> GetByChannelIdAsync(
        Guid channelId,
        Guid userId,
        MessageQueryDto query)
    {
        var hasAccess = await _dbContext.Channels
            .AnyAsync(c =>
                c.Id == channelId &&
                c.Workspace.Members.Any(
                    m => m.UserId == userId));

        if (!hasAccess)
        {
            throw new NotFoundException("Channel not found");
        }

        var totalCount = await _dbContext.Messages
            .CountAsync(m =>
                m.ChannelId == channelId);

        var messages = await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ChannelId == channelId)
            .OrderBy(m => m.CreatedAt)
            .Skip(
                (query.PageNumber - 1)
                * query.PageSize)
            .Take(query.PageSize)
            .Select(m => new MessageResponseDto
            {
                Id = m.Id,
                ChannelId = m.ChannelId,
                UserId = m.UserId,
                Username = m.User.Username,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

        return new PagedResult<MessageResponseDto>
        {
            Items = messages,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }
}