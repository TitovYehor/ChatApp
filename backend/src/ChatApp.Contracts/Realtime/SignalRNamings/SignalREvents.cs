namespace ChatApp.Contracts.Realtime.SignalRNamings;

public static class SignalREvents
{
    public const string MessageCreated =
        nameof(MessageCreated);

    public const string MessageUpdated =
        nameof(MessageUpdated);

    public const string MessageDeleted =
        nameof(MessageDeleted);

    public const string UserPresenceChanged =
        nameof(UserPresenceChanged);

    public const string OnlineUsersSnapshot =
        nameof(OnlineUsersSnapshot);

    public const string UserTyping =
        nameof(UserTyping);

    public const string WorkspaceDeleted =
        nameof(WorkspaceDeleted);

    public const string WorkspaceUpdated = 
        nameof(WorkspaceUpdated);
}