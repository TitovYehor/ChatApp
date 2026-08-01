namespace ChatApp.Contracts.Realtime.SignalRNamings;

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