using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Users.Requests;
using ChatApp.Contracts.Users.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileResponseDto> GetProfileAsync(
        Guid userId)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return ToProfileDto(user);
    }

    public async Task<UserProfileResponseDto> UpdateProfileAsync(
        Guid userId,
        UpdateUserProfileRequestDto request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Id == userId);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        var username = request.Username.Trim();

        var usernameTaken = await _dbContext.Users
            .AnyAsync(x =>
                x.Id != userId &&
                x.Username == username);

        if (usernameTaken)
        {
            throw new UserAlreadyExistsException();
        }

        user.Username = username;
        
        await _dbContext.SaveChangesAsync();

        return ToProfileDto(user);
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequestDto request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Id == userId);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        var currentPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.CurrentPassword,
            user.PasswordHash);

        if (!currentPasswordValid)
        {
            throw new InvalidCredentialsException();
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
            request.NewPassword);

        await RevokeRefreshTokensAsync(userId);

        await _dbContext.SaveChangesAsync();
    }

    private async Task RevokeRefreshTokensAsync(
        Guid userId)
    {
        var activeRefreshTokens = await _dbContext.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ToListAsync();

        var revokedAt = DateTime.UtcNow;

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.RevokedAt = revokedAt;
        }
    }

    private static UserProfileResponseDto ToProfileDto(
        User user)
    {
        return new UserProfileResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };
    }
}