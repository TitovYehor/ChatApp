using ChatApp.Contracts.Common;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappings;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly AppDbContext _dbContext;

    private readonly IChatNotifier _chatNotifier;

    private readonly INotificationService _notificationService;

    public MessageService(
        AppDbContext dbContext,
        IChatNotifier chatNotifier,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _chatNotifier = chatNotifier;
        _notificationService = notificationService;
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

        var response = message.ToDto();

        await _chatNotifier.MessageCreatedAsync(
            message.ChannelId,
            response);

        var memberIds = channel.Workspace.Members
            .Select(x => x.UserId)
            .ToList();

        var actorUsername = response.Username;

        await NotifyWorkspaceMembersAsync(
            memberIds,
            userId,
            "MessageCreated",
            $"{actorUsername} sent a message in #{channel.Name}",
            channel.WorkspaceId,
            message.Id);

        return response;
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

        var messagesQuery = _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ChannelId == channelId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            messagesQuery = messagesQuery.Where(m =>
                m.Content.ToLower().Contains(search));
        }

        var totalCount = await messagesQuery.CountAsync();

        var messages = await messagesQuery
            .OrderBy(m => m.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
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

    public async Task<MessageResponseDto> UpdateAsync(
        Guid messageId,
        Guid userId,
        UpdateMessageRequestDto request)
    {
        var message = await _dbContext.Messages
            .Include(x => x.User)
            .Include(x => x.Channel)
                .ThenInclude(x => x.Workspace)
                .ThenInclude(x => x.Members)
            .FirstOrDefaultAsync(x =>
                x.Id == messageId);

        if (message == null)
        {
            throw new NotFoundException("Message not found");
        }

        var isOwner = message.UserId == userId;

        if (!isOwner)
        {
            throw new ForbiddenException("You can only edit your own messages");
        }

        var content = request.Content.Trim();

        if (message.Content == content)
        {
            return message.ToDto();
        }

        message.Content = content;
        message.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var response = message.ToDto();

        await _chatNotifier.MessageUpdatedAsync(
            message.ChannelId,
            response);

        var memberIds = message.Channel.Workspace.Members
            .Select(x => x.UserId)
            .ToList();

        await NotifyWorkspaceMembersAsync(
            memberIds,
            userId,
            "MessageUpdated",
            $"{message.User.Username} updated a message in #{message.Channel.Name}",
            message.Channel.WorkspaceId,
            message.Id);

        return response;
    }

    public async Task DeleteAsync(
        Guid messageId,
        Guid userId)
    {
        var message = await _dbContext.Messages
            .Include(x => x.Channel)
                .ThenInclude(x => x.Workspace)
                .ThenInclude(x => x.Members)
            .FirstOrDefaultAsync(x =>
                x.Id == messageId);

        if (message == null)
        {
            throw new NotFoundException("Message not found");
        }

        var member = message.Channel.Workspace.Members
            .FirstOrDefault(x =>
                x.UserId == userId);

        if (member == null)
        {
            throw new ForbiddenException("User is not a member of this workspace");
        }

        var isOwner = message.UserId == userId;

        var isWorkspaceAdmin = 
            member.Role == WorkspaceRole.Admin ||
            member.Role == WorkspaceRole.Owner;

        if (!isOwner && !isWorkspaceAdmin)
        {
            throw new ForbiddenException("You do not have permission to delete this message");
        }

        var channelId = message.ChannelId;

        var workspaceId = message.Channel.WorkspaceId;
        var channelName = message.Channel.Name;

        var memberIds = message.Channel.Workspace.Members
            .Select(x => x.UserId)
            .ToList();

        var actorUsername = await _dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Username)
            .SingleAsync();

        _dbContext.Messages.Remove(message);

        await _dbContext.SaveChangesAsync();

        await _chatNotifier.MessageDeletedAsync(
            channelId,
            new MessageDeletedResponseDto
            {
                MessageId = messageId,
                ChannelId = channelId
            });

        await NotifyWorkspaceMembersAsync(
            memberIds,
            userId,
            "MessageDeleted",
            $"{actorUsername} deleted a message in #{channelName}",
            workspaceId,
            messageId);
    }

    private async Task NotifyWorkspaceMembersAsync(
        IReadOnlyCollection<Guid> memberIds,
        Guid actorUserId,
        string type,
        string message,
        Guid workspaceId,
        Guid entityId)
    {
        var recipients = memberIds
            .Distinct()
            .ToList();

        foreach (var recipientId in recipients)
        {
            await _notificationService.CreateAsync(
                recipientId,
                actorUserId,
                type,
                message,
                workspaceId,
                entityId);
        }
    }
}