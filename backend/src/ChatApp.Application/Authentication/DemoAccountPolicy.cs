namespace ChatApp.Application.Authentication;

/// <summary>
/// Defines the identity and restrictions of the application's demo account
/// </summary>
public static class DemoAccountPolicy
{
    /// <summary>
    /// Gets the email address used by the demo account
    /// </summary>
    public const string Email = "demo@example.com";

    /// <summary>
    /// Gets the message explaining the demo account's profile restrictions
    /// </summary>
    public const string RestrictionMessage = "The demo account cannot change its username or password";

    /// <summary>
    /// Determines whether the specified email belongs to the demo account
    /// </summary>
    public static bool IsDemoAccount(string? email)
    {
        return string.Equals(
            email,
            Email,
            StringComparison.OrdinalIgnoreCase);
    }
}