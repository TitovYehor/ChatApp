namespace ChatApp.SignalRTester.UI.Models;

public class MenuItem
{
    public int Number { get; init; }

    public string Text { get; init; } = string.Empty;

    public MenuOption Option { get; init; }

    public bool Visible { get; init; } = true;
}