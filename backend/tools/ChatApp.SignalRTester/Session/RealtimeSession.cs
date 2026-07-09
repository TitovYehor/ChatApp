namespace ChatApp.SignalRTester.Session;

public class RealtimeSession
{
    public bool IsConnected { get; private set; }


    public void Connected()
    {
        IsConnected = true;
    }


    public void Disconnected()
    {
        IsConnected = false;
    }
}