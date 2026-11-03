namespace ChatApp.Application.Exceptions;

/// <summary>
/// Represents an operation that the current user is not authorized to perform
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message)
        : base(message)
    {
    }
}