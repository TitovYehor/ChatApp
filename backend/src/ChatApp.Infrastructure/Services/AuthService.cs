using ChatApp.Application.Authentication;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.Contracts.Users;
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

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(
            request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        var accessToken = GenerateJwtToken(user);

        var refreshToken = await CreateRefreshTokenAsync(user);

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

    public async Task<AuthResult> LoginAsync(
        LoginRequestDto request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Email == request.Email);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            throw new InvalidCredentialsException();
        }

        var accessToken = GenerateJwtToken(user);

        var refreshToken = await CreateRefreshTokenAsync(user);

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

    public async Task<AuthResult> RefreshAsync(
        string refreshToken)
    {
        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash);

        if (storedToken is null)
        {
            throw new InvalidCredentialsException();
        }

        if (!storedToken.IsActive)
        {
            throw new InvalidCredentialsException();
        }

        var user = storedToken.User;

        storedToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashRefreshToken(
                newRefreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays),
        };

        _dbContext.RefreshTokens.Add(newRefreshTokenEntity);

        var newAccessToken = GenerateJwtToken(user);

        await _dbContext.SaveChangesAsync();

        return new AuthResult
        {
            Response = new AuthResponseDto
            {
                AccessToken = newAccessToken,
                User = new AuthenticatedUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                }
            },
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(
        string refreshToken)
    {
        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash);

        if (storedToken is null)
        {
            return;
        }

        if (storedToken.RevokedAt is not null)
        {
            return;
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    private string GenerateJwtToken(
        User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
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

    private async Task<string> CreateRefreshTokenAsync(
        User user)
    {
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = HashRefreshToken(refreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays),
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);

        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    private static string HashRefreshToken(
        string refreshToken)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(
                refreshToken));

        return Convert.ToHexString(bytes);
    }
}