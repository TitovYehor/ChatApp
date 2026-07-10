namespace ChatApp.SignalRTester.Session;

public class RealtimeSession
{
    public bool IsConnected { get; private set; }


    public void MarkConnected()
    {
        IsConnected = true;
    }


    public void MarkDisconnected()
    {
        IsConnected = false;
    }
}