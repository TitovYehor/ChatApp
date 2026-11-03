namespace ChatApp.Application.Exceptions;

/// <summary>
/// Represents a request for an application resource that does not exist or cannot be found
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}