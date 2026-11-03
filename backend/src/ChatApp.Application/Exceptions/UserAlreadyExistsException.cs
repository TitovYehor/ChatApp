namespace ChatApp.Application.Exceptions;

/// <summary>
/// Represents an attempt to register a user with an email or username that is already in use
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException()
        : base("User with this email or username already exists")
    {
    }
}