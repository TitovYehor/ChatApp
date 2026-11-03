namespace ChatApp.Application.Exceptions;

/// <summary>
/// Represents an authentication attempt with invalid credentials
/// </summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid credentials")
    {
    }
}