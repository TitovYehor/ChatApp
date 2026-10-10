using ChatApp.Application.Authentication;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.Contracts.Users;
using ChatApp.Contracts.Users.Requests;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Authentication;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        AppDbContext dbContext,
        IOptions<JwtSettings> jwtOptions)
    {
        _dbContext = dbContext;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResult> RegisterAsync(
        RegisterRequestDto request)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Email == request.Email ||
                x.Username == request.Username);

        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException();
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.Password)
        };

        _dbContext.Users.Add(user);

        var accessToken = GenerateJwtToken(user);
        var refreshToken = CreateRefreshToken(user);

        await _dbContext.SaveChangesAsync();

        return CreateAuthResult(user, accessToken, refreshToken);
    }

    public async Task<AuthResult> LoginAsync(
        LoginRequestDto request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user is null ||
            !BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = CreateRefreshToken(user);

        await _dbContext.SaveChangesAsync();

        return CreateAuthResult(user, accessToken, refreshToken);
    }

    public async Task<AuthResult> RefreshAsync(
        string refreshToken)
    {
        var tokenHash = RefreshTokenHasher.Hash(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (storedToken is null || !storedToken.IsActive)
        {
            throw new InvalidCredentialsException();
        }

        var user = storedToken.User;
        storedToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = CreateRefreshToken(user);
        var newAccessToken = GenerateJwtToken(user);

        await _dbContext.SaveChangesAsync();

        return CreateAuthResult(
            user,
            newAccessToken,
            newRefreshToken);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenHash = RefreshTokenHasher.Hash(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (storedToken is null ||
            storedToken.RevokedAt is not null)
        {
            return;
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<AuthResult> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequestDto request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

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

        var existingRefreshTokens = await _dbContext.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ToListAsync();

        var revokedAt = DateTime.UtcNow;

        foreach (var token in existingRefreshTokens)
        {
            token.RevokedAt = revokedAt;
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = CreateRefreshToken(user);

        await _dbContext.SaveChangesAsync();

        return CreateAuthResult(user, accessToken, refreshToken);
    }

    private AuthResult CreateAuthResult(
        User user,
        string accessToken,
        string refreshToken)
    {
        return new AuthResult
        {
            Response = new AuthResponseDto
            {
                AccessToken = accessToken,
                User = new AuthenticatedUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                }
            },
            RefreshToken = refreshToken
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,
                user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            _jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string CreateRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = RefreshTokenHasher.Hash(refreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays)
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);

        return refreshToken;
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}