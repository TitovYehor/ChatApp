namespace ChatApp.Application.Realtime;

public static class SignalRGroups
{
    public static string Channel(
        Guid channelId)
    {
        return $"channel-{channelId}";
    }
}