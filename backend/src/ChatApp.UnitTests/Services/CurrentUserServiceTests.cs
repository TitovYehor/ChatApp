using ChatApp.Application.Exceptions;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace ChatApp.UnitTests.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

    public CurrentUserServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    }

    [Fact]
    public void GetUserId_WhenValidUserIdClaimExists_ReturnsUserId()
    {
        var userId = Guid.NewGuid();

        SetUser(new ClaimsPrincipal(
                    new ClaimsIdentity(
                    [
                        new Claim(
                            ClaimTypes.NameIdentifier,
                            userId.ToString())
                    ],
                    "TestAuth")));

        var service = CreateService();

        var result = service.GetUserId();

        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_WhenUserIdClaimIsMissing_ThrowsUserNotAuthenticatedException()
    {
        SetUser(new ClaimsPrincipal(new ClaimsIdentity()));

        var service = CreateService();

        var exception = Assert.Throws<UserNotAuthenticatedException>(
            () => service.GetUserId());

        Assert.Equal(
            "User is not authenticated",
            exception.Message);
    }

    [Fact]
    public void GetUserId_WhenUserIdClaimIsInvalid_ThrowsUserNotAuthenticatedException()
    {
        SetUser(new ClaimsPrincipal(
                    new ClaimsIdentity(
                    [
                        new Claim(
                            ClaimTypes.NameIdentifier,
                            "not-a-guid")
                    ],
                    "TestAuth")));

        var service = CreateService();

        var exception = Assert.Throws<UserNotAuthenticatedException>(
            () => service.GetUserId());

        Assert.Equal(
            "User is not authenticated",
            exception.Message);
    }

    private CurrentUserService CreateService()
    {
        return new CurrentUserService(
            _httpContextAccessorMock.Object);
    }

    private void SetUser(ClaimsPrincipal user)
    {
        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(httpContext);
    }
}