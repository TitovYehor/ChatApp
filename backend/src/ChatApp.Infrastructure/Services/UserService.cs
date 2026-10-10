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