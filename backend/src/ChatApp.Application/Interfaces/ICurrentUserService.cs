namespace ChatApp.Application.Interfaces;

/// <summary>
/// Provides access to the identifier of the currently authenticated user
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Retrieves the identifier of the currently authenticated user
    /// </summary>
    Guid GetUserId();
}