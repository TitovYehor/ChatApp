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

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException();
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            AccessToken = token,
            User = new AuthenticatedUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

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

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            AccessToken = token,
            User = new AuthenticatedUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            }
        };
    }

    private string GenerateJwtToken(User user)
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
}