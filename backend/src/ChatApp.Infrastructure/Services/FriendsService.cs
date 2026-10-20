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

    private readonly IFriendshipNotifier _friendshipNotifier;

    private readonly INotificationService _notificationService;

    public FriendsService(
        AppDbContext dbContext,
        IFriendshipNotifier friendshipNotifier,
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _friendshipNotifier = friendshipNotifier;
        _notificationService = notificationService;
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

        var (firstUserId, secondUserId) = OrderIds(
            currentUserId,
            friendUserId);

        var users = await _dbContext.Users
            .AsNoTracking()
            .Where(x =>
                x.Id == currentUserId ||
                x.Id == friendUserId)
            .Select(x => new
            {
                x.Id,
                x.Username
            })
            .ToDictionaryAsync(x => x.Id, x => x.Username);

        if (!users.TryGetValue(friendUserId, out var friendUsername))
        {
            throw new NotFoundException("User was not found");
        }

        if (!users.TryGetValue(currentUserId, out var actorUsername))
        {
            throw new NotFoundException("Current user was not found");
        }

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

        await _friendshipNotifier.FriendshipChangedAsync(
            new[] { currentUserId, friendUserId });

        await _notificationService.CreateAsync(
            currentUserId,
            friendUserId,
            "FriendAdded",
            $"You and {friendUsername} are now friends");

        await _notificationService.CreateAsync(
            friendUserId,
            currentUserId,
            "FriendAdded",
            $"{actorUsername} added you as a friend");
    }

    public async Task RemoveFriendAsync(
        Guid currentUserId,
        Guid friendUserId)
    {
        if (currentUserId == friendUserId)
        {
            throw new ConflictException("You cannot remove yourself from your friends list");
        }

        var (firstUserId, secondUserId) = OrderIds(
            currentUserId,
            friendUserId);

        var friendship = await _dbContext.Friendships
            .FirstOrDefaultAsync(x =>
                x.UserId == firstUserId &&
                x.FriendUserId == secondUserId);

        if (friendship == null)
        {
            throw new NotFoundException("Friendship was not found");
        }

        var usernames = await _dbContext.Users
            .AsNoTracking()
            .Where(x =>
                x.Id == currentUserId ||
                x.Id == friendUserId)
            .Select(x => new
            {
                x.Id,
                x.Username
            })
            .ToDictionaryAsync(x => x.Id, x => x.Username);

        var actorUsername = usernames[currentUserId];
        var friendUsername = usernames[friendUserId];

        _dbContext.Friendships.Remove(friendship);

        await _dbContext.SaveChangesAsync();

        await _friendshipNotifier.FriendshipChangedAsync(
            new[] { currentUserId, friendUserId });

        await _notificationService.CreateAsync(
            currentUserId,
            friendUserId,
            "FriendRemoved",
            $"You removed {friendUsername} from your friends");

        await _notificationService.CreateAsync(
            friendUserId,
            currentUserId,
            "FriendRemoved",
            $"{actorUsername} removed you from their friends");
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