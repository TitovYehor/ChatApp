namespace ChatApp.Application.Realtime;

public static class SignalRGroups
{
    public static string ChannelGroup(
        Guid channelId)
    {
        return $"channel-{channelId}";
    }

    public static string UserGroup(Guid userId)
    {
        return $"user-{userId}";
    }
}