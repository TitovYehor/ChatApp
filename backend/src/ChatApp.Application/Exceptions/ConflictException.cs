namespace ChatApp.Application.Exceptions;

/// <summary>
/// Represents an operation that cannot be completed because it conflicts with the current application state
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}