namespace ChatApp.Application.Realtime;

/// <summary>
/// Defines consistent SignalR group names for channels, users, and private conversations
/// </summary>
public static class SignalRGroups
{
    /// <summary>
    /// Creates the SignalR group name for a channel
    /// </summary>
    public static string ChannelGroup(
        Guid channelId)
    {
        return $"channel-{channelId}";
    }

    /// <summary>
    /// Creates the SignalR group name for a user
    /// </summary>
    public static string UserGroup(Guid userId)
    {
        return $"user-{userId}";
    }

    /// <summary>
    /// Creates the SignalR group name for a private conversation
    /// </summary>
    public static string PrivateConversationGroup(Guid conversationId)
    {
        return $"private-conversation-{conversationId}";
    }
}