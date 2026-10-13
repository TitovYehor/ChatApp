using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Friends.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class FriendsService : IFriendsService
{
    private readonly AppDbContext _dbContext;

    public FriendsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<FriendResponseDto>> GetFriendsAsync(
        Guid currentUserId)
    {
        var friendships = await _dbContext.Friendships
            .AsNoTracking()
            .Where(x =>
                x.UserId == currentUserId ||
                x.FriendUserId == currentUserId)
            .Select(x => new
            {
                FriendId = x.UserId == currentUserId
                    ? x.FriendUserId
                    : x.UserId,
                x.CreatedAt
            })
            .Join(
                _dbContext.Users.AsNoTracking(),
                friendship => friendship.FriendId,
                user => user.Id,
                (friendship, user) => new FriendResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    CreatedAt = friendship.CreatedAt
                })
            .OrderBy(x => x.Username)
            .ToListAsync();

        return friendships;
    }

    public async Task<IReadOnlyCollection<FriendSearchResponseDto>> SearchUsersAsync(
        Guid currentUserId,
        string query)
    {
        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<FriendSearchResponseDto>();
        }

        var friendIds = await _dbContext.Friendships
            .AsNoTracking()
            .Where(x =>
                x.UserId == currentUserId ||
                x.FriendUserId == currentUserId)
            .Select(x => x.UserId == currentUserId
                ? x.FriendUserId
                : x.UserId)
            .ToListAsync();

        var pattern = $"%{EscapeLikePattern(query)}%";

        var users = await _dbContext.Users
            .AsNoTracking()
            .Where(x =>
                x.Id != currentUserId &&
                EF.Functions.ILike(x.Username, pattern))
            .OrderBy(x => x.Username)
            .Take(20)
            .Select(x => new
            {
                x.Id,
                x.Username
            })
            .ToListAsync();

        var friendIdSet = friendIds.ToHashSet();

        return users
            .Select(x => new FriendSearchResponseDto
            {
                UserId = x.Id,
                Username = x.Username,
                IsFriend = friendIdSet.Contains(x.Id)
            })
            .ToList();
    }

    public async Task AddFriendAsync(
        Guid currentUserId,
        Guid friendUserId)
    {
        if (currentUserId == friendUserId)
        {
            throw new ConflictException("You cannot add yourself as a friend");
        }

        var targetExists = await _dbContext.Users
            .AnyAsync(x => x.Id == friendUserId);

        if (!targetExists)
        {
            throw new NotFoundException("User was not found");
        }

        var (firstUserId, secondUserId) = OrderIds(
            currentUserId,
            friendUserId);

        var friendshipExists = await _dbContext.Friendships
            .AnyAsync(x =>
                x.UserId == firstUserId &&
                x.FriendUserId == secondUserId);

        if (friendshipExists)
        {
            throw new ConflictException("You are already friends with this user");
        }

        _dbContext.Friendships.Add(new Friendship
        {
            Id = Guid.NewGuid(),
            UserId = firstUserId,
            FriendUserId = secondUserId,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }

    private static (Guid First, Guid Second) OrderIds(
        Guid first,
        Guid second)
    {
        return first.CompareTo(second) < 0
            ? (first, second)
            : (second, first);
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}