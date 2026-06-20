namespace ChatApp.Contracts.Realtime;

public static class SignalREvents
{
    public const string MessageCreated =
        nameof(MessageCreated);

    public const string MessageUpdated =
        nameof(MessageUpdated);

    public const string MessageDeleted =
        nameof(MessageDeleted);
}