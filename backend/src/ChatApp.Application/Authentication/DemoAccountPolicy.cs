namespace ChatApp.Application.Authentication;

public static class DemoAccountPolicy
{
    public const string Email = "demo@example.com";

    public const string RestrictionMessage = "The demo account cannot change its username or password";

    public static bool IsDemoAccount(string? email)
    {
        return string.Equals(
            email,
            Email,
            StringComparison.OrdinalIgnoreCase);
    }
}