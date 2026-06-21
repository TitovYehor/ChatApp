namespace ChatApp.SignalRTester.UI;

public class ConsoleMenu : IConsoleMenu
{
    public Task RunAsync()
    {
        Console.Clear();

        Console.WriteLine("==================================");
        Console.WriteLine("      ChatApp SignalR Tester      ");
        Console.WriteLine("==================================");

        return Task.CompletedTask;
    }
}