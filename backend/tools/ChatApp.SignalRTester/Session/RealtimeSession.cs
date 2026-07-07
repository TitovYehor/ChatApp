namespace ChatApp.SignalRTester.Session;

public class RealtimeSession
{
    public bool IsConnected { get; private set; }

    public Guid? CurrentChannelId { get; private set; }

    public void Connected()
    {
        IsConnected = true;
    }

    public void Disconnected()
    {
        IsConnected = false;
        CurrentChannelId = null;
    }

    public void JoinedChannel(
        Guid channelId)
    {
        CurrentChannelId = channelId;
    }

    public void LeftChannel()
    {
        CurrentChannelId = null;
    }
}