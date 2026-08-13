using ChatApp.Application.Exceptions;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Authentication;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.UnitTests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_NewUser_CreatesUserAndReturnsAuthenticationData()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateAuthService(dbContext);

        var request = new RegisterRequestDto
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "Password123"
        };

        var result = await service.RegisterAsync(request);

        Assert.NotEqual(Guid.Empty, result.User.Id);
        Assert.Equal("newuser", result.User.Username);
        Assert.Equal("new@example.com", result.User.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));

        var savedUser = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == result.User.Id);

        Assert.NotNull(savedUser);
        Assert.Equal("newuser", savedUser.Username);
        Assert.Equal("new@example.com", savedUser.Email);
        Assert.NotEqual(
            request.Password,
            savedUser.PasswordHash);

        Assert.True(
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                savedUser.PasswordHash));

        var jwtSettings = CreateJwtSettings();

        var principal = ValidateToken(
            result.AccessToken,
            jwtSettings);

        Assert.Equal(
            result.User.Id.ToString(),
            principal.FindFirstValue(
                ClaimTypes.NameIdentifier));

        Assert.Equal(
            result.User.Username,
            principal.FindFirstValue(
                ClaimTypes.Name));

        Assert.Equal(
            result.User.Email,
            principal.FindFirstValue(
                ClaimTypes.Email));
    }

    [Fact]
    public async Task RegisterAsync_ExistingEmail_ThrowsUserAlreadyExistsException()
    {
        await using var dbContext = CreateDbContext();

        var existingUser = CreateUser(
            username: "existinguser",
            email: "existing@example.com");

        dbContext.Users.Add(existingUser);

        await dbContext.SaveChangesAsync();

        var service = CreateAuthService(dbContext);

        var request = new RegisterRequestDto
        {
            Username = "differentuser",
            Email = "existing@example.com",
            Password = "Password123"
        };

        var exception = await Assert.ThrowsAsync<UserAlreadyExistsException>(
            () => service.RegisterAsync(request));

        Assert.Equal(
            "User with this email or username already exists",
            exception.Message);

        Assert.Single(await dbContext.Users.ToListAsync());
    }

    [Fact]
    public async Task RegisterAsync_ExistingUsername_ThrowsUserAlreadyExistsException()
    {
        await using var dbContext = CreateDbContext();

        var existingUser = CreateUser(
            username: "existinguser",
            email: "existing@example.com");

        dbContext.Users.Add(existingUser);

        await dbContext.SaveChangesAsync();

        var service = CreateAuthService(dbContext);

        var request = new RegisterRequestDto
        {
            Username = "existinguser",
            Email = "different@example.com",
            Password = "Password123"
        };

        var exception = await Assert.ThrowsAsync<UserAlreadyExistsException>(
            () => service.RegisterAsync(request));

        Assert.Equal(
            "User with this email or username already exists",
            exception.Message);

        Assert.Single(await dbContext.Users.ToListAsync());
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static AuthService CreateAuthService(
        AppDbContext dbContext)
    {
        var jwtSettings = new JwtSettings
        {
            SecretKey =
                "test-secret-key-that-is-long-enough-for-hmac-sha256",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationMinutes = 60
        };

        return new AuthService(
            dbContext,
            Options.Create(jwtSettings));
    }

    private static User CreateUser(
        string username = "testuser",
        string email = "test@example.com",
        string password = "Password123")
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
    }

    private static JwtSettings CreateJwtSettings()
    {
        return new JwtSettings
        {
            SecretKey =
                "test-secret-key-that-is-long-enough-for-hmac-sha256",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationMinutes = 60
        };
    }

    private static ClaimsPrincipal ValidateToken(
        string token,
        JwtSettings settings)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            settings.SecretKey)),

                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,

                ValidateAudience = true,
                ValidAudience = settings.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

        return tokenHandler.ValidateToken(
            token,
            validationParameters,
            out _);
    }
}