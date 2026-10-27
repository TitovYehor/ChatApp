using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Common;
using ChatApp.Contracts.Messages.Requests;
using ChatApp.Contracts.PrivateConversations.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public sealed class PrivateConversationService : IPrivateConversationService
{
    private readonly AppDbContext _dbContext;

    private readonly IChatNotifier _chatNotifier;

    public PrivateConversationService(
        AppDbContext dbContext,
        IChatNotifier chatNotifier)
    {
        _dbContext = dbContext;
        _chatNotifier = chatNotifier;
    }

    public async Task<PrivateConversationResponseDto> GetOrCreateAsync(
        Guid currentUserId,
        Guid otherUserId)
    {
        if (currentUserId == otherUserId)
        {
            throw new ConflictException("You cannot start a conversation with yourself");
        }

        var otherUserExists = await _dbContext.Users
            .AnyAsync(x => x.Id == otherUserId);

        if (!otherUserExists)
        {
            throw new NotFoundException("User was not found");
        }

        if (!await HasFriendshipAsync(currentUserId, otherUserId))
        {
            throw new ForbiddenException("You can only start a conversation with a friend");
        }

        var (user1Id, user2Id) = OrderIds(
            currentUserId,
            otherUserId);

        var conversation = await _dbContext.PrivateConversations
            .FirstOrDefaultAsync(x =>
                x.User1Id == user1Id &&
                x.User2Id == user2Id);

        if (conversation is null)
        {
            conversation = new PrivateConversation
            {
                Id = Guid.NewGuid(),
                User1Id = user1Id,
                User2Id = user2Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.PrivateConversations.Add(conversation);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _dbContext.Entry(conversation).State = EntityState.Detached;

                conversation = await _dbContext.PrivateConversations
                    .FirstOrDefaultAsync(x =>
                        x.User1Id == user1Id &&
                        x.User2Id == user2Id);

                if (conversation is null)
                {
                    throw;
                }
            }
        }

        return await ProjectConversation(
                conversation.Id,
                currentUserId)
            ?? throw new NotFoundException("Conversation was not found");
    }

    public async Task<IReadOnlyCollection<PrivateConversationResponseDto>>
        GetAllAsync(Guid currentUserId)
    {
        return await _dbContext.PrivateConversations
            .AsNoTracking()
            .Where(x =>
                x.User1Id == currentUserId ||
                x.User2Id == currentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PrivateConversationResponseDto
            {
                Id = x.Id,
                OtherUserId = x.User1Id == currentUserId
                    ? x.User2Id
                    : x.User1Id,
                OtherUsername = x.User1Id == currentUserId
                    ? x.User2.Username
                    : x.User1.Username,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<PagedResult<PrivateMessageResponseDto>> GetMessagesAsync(
        Guid currentUserId,
        Guid conversationId,
        MessageQueryDto query)
    {
        await EnsureParticipantAsync(currentUserId, conversationId);

        var messages = _dbContext.PrivateMessages
            .AsNoTracking()
            .Where(x => x.ConversationId == conversationId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";

            messages = messages.Where(x =>
                EF.Functions.ILike(x.Content, pattern));
        }

        var totalCount = await messages.CountAsync();

        var items = await messages
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new PrivateMessageResponseDto
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                UserId = x.UserId,
                Username = x.User.Username,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        items.Reverse();

        return new PagedResult<PrivateMessageResponseDto>
        {
            Items = items,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PrivateMessageResponseDto> SendMessageAsync(
        Guid currentUserId,
        Guid conversationId,
        CreateMessageRequestDto request)
    {
        var conversation = await _dbContext.PrivateConversations
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == conversationId &&
                (x.User1Id == currentUserId ||
                 x.User2Id == currentUserId));

        if (conversation is null)
        {
            throw new NotFoundException("Conversation was not found");
        }

        var otherUserId = conversation.User1Id == currentUserId
            ? conversation.User2Id
            : conversation.User1Id;

        if (!await HasFriendshipAsync(currentUserId, otherUserId))
        {
            throw new ForbiddenException("You must be friends to send messages");
        }

        var content = request.Content.Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ConflictException("Message content cannot be empty");
        }

        var message = new PrivateMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            UserId = currentUserId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.PrivateMessages.Add(message);

        await _dbContext.SaveChangesAsync();

        var username = await _dbContext.Users
            .Where(x => x.Id == currentUserId)
            .Select(x => x.Username)
            .SingleAsync();

        var response = new PrivateMessageResponseDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            UserId = message.UserId,
            Username = username,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };

        await _chatNotifier.PrivateMessageCreatedAsync(
            conversationId,
            response);

        return response;
    }

    public async Task<PrivateMessageResponseDto> UpdateMessageAsync(
        Guid currentUserId,
        Guid conversationId,
        Guid messageId,
        UpdateMessageRequestDto request)
    {
        var message = await _dbContext.PrivateMessages
            .FirstOrDefaultAsync(x =>
                x.Id == messageId &&
                x.ConversationId == conversationId);

        if (message is null)
        {
            throw new NotFoundException("Private message was not found");
        }

        if (message.UserId != currentUserId)
        {
            throw new ForbiddenException("You can only edit your own messages");
        }

        var content = request.Content.Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ConflictException("Message content cannot be empty");
        }

        if (message.Content == content)
        {
            return await ProjectPrivateMessageAsync(message);
        }

        message.Content = content;
        message.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await ProjectPrivateMessageAsync(message);
    }

    public async Task DeleteMessageAsync(
        Guid currentUserId,
        Guid conversationId,
        Guid messageId)
    {
        var message = await _dbContext.PrivateMessages
            .FirstOrDefaultAsync(x =>
                x.Id == messageId &&
                x.ConversationId == conversationId);

        if (message is null)
        {
            throw new NotFoundException("Private message was not found");
        }

        if (message.UserId != currentUserId)
        {
            throw new ForbiddenException("You can only delete your own messages");
        }

        _dbContext.PrivateMessages.Remove(message);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsParticipantAsync(
        Guid userId,
        Guid conversationId)
    {
        return await _dbContext.PrivateConversations
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == conversationId &&
                (x.User1Id == userId || x.User2Id == userId));
    }

    private async Task EnsureParticipantAsync(
        Guid currentUserId,
        Guid conversationId)
    {
        var isParticipant = await _dbContext.PrivateConversations
            .AnyAsync(x =>
                x.Id == conversationId &&
                (x.User1Id == currentUserId ||
                 x.User2Id == currentUserId));

        if (!isParticipant)
        {
            throw new NotFoundException("Conversation was not found");
        }
    }

    private async Task<bool> HasFriendshipAsync(
        Guid firstUserId,
        Guid secondUserId)
    {
        var (user1Id, user2Id) = OrderIds(
            firstUserId,
            secondUserId);

        return await _dbContext.Friendships
            .AnyAsync(x =>
                x.UserId == user1Id &&
                x.FriendUserId == user2Id);
    }

    private async Task<PrivateConversationResponseDto?> ProjectConversation(
        Guid conversationId,
        Guid currentUserId)
    {
        return await _dbContext.PrivateConversations
            .AsNoTracking()
            .Where(x =>
                x.Id == conversationId &&
                (x.User1Id == currentUserId ||
                 x.User2Id == currentUserId))
            .Select(x => new PrivateConversationResponseDto
            {
                Id = x.Id,
                OtherUserId = x.User1Id == currentUserId
                    ? x.User2Id
                    : x.User1Id,
                OtherUsername = x.User1Id == currentUserId
                    ? x.User2.Username
                    : x.User1.Username,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    private async Task<PrivateMessageResponseDto> ProjectPrivateMessageAsync(
        PrivateMessage message)
    {
        var username = await _dbContext.Users
            .Where(x => x.Id == message.UserId)
            .Select(x => x.Username)
            .SingleAsync();

        return new PrivateMessageResponseDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            UserId = message.UserId,
            Username = username,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };
    }

    private static (Guid User1Id, Guid User2Id) OrderIds(
        Guid firstUserId,
        Guid secondUserId)
    {
        return firstUserId.CompareTo(secondUserId) < 0
            ? (firstUserId, secondUserId)
            : (secondUserId, firstUserId);
    }
}