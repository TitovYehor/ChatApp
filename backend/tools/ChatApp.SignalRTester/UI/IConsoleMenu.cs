using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.UI;

public interface IConsoleMenu
{
    Task<MenuOption?> ShowAsync();
}