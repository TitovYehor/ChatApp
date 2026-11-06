namespace ChatApp.Contracts.Realtime.SignalRNamings;

/// <summary>
/// Defines the method names clients invoke to perform realtime actions on the server
/// </summary>
public static class SignalRMethods
{
    public const string JoinChannel =
        nameof(JoinChannel);

    public const string LeaveChannel =
        nameof(LeaveChannel);

    public const string TypingStarted =
        nameof(TypingStarted);

    public const string TypingStopped =
        nameof(TypingStopped);
}