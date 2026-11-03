namespace ChatApp.Application.Exceptions;

/// <summary>
/// Represents an operation that requires an authenticated user when no authenticated user is available
/// </summary>
public class UserNotAuthenticatedException : Exception
{
    public UserNotAuthenticatedException()
        : base("User is not authenticated")
    {
    }
}