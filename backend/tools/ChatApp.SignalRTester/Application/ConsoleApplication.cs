using ChatApp.Contracts.Authentication.Requests;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.Application;

public class ConsoleApplication : IConsoleApplication
{
    private readonly IConsoleMenu _menu;

    private readonly IAuthenticationApiClient _authenticationApiClient;

    private readonly IConsoleInput _consoleInput;

    private readonly UserSession _userSession;

    public ConsoleApplication(
        IConsoleMenu menu,
        IConsoleInput consoleInput,
        IAuthenticationApiClient authenticationApiClient,
        UserSession userSession)
    {
        _menu = menu;
        _consoleInput = consoleInput;
        _authenticationApiClient = authenticationApiClient;
        _userSession = userSession;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            var option = await _menu.ShowAsync();

            if (option == null)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid selection");
                Console.WriteLine();

                continue;
            }

            switch (option.Value)
            {
                case MenuOption.Login:
                    await LoginAsync();
                    break;

                case MenuOption.Logout:
                    Logout();
                    break;

                case MenuOption.Exit:
                    return;
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER...");
            Console.ReadLine();
        }
    }

    private async Task LoginAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== Login ===");
        Console.WriteLine();

        var email = _consoleInput.ReadRequiredString("Email");

        var password = _consoleInput.ReadRequiredString("Password");

        var request = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        var result = await _authenticationApiClient.LoginAsync(request);

        if (!result.IsSuccess)
        {
            Console.WriteLine();
            Console.WriteLine(result.ErrorMessage);

            return;
        }

        var response = result.Data!;

        _userSession.SignIn(response);

        Console.WriteLine();
        Console.WriteLine("Login successful");
        Console.WriteLine();

        Console.WriteLine($"Welcome {_userSession.Username}!");
        Console.WriteLine($"Email: {_userSession.Email}");
    }

    private void Logout()
    {
        _userSession.SignOut();

        Console.WriteLine();
        Console.WriteLine("Logged out successfully");
    }
}