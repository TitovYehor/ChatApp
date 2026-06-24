using ChatApp.Contracts.Authentication.Requests;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.UI;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Models;

namespace ChatApp.SignalRTester.Application;

public class ConsoleApplication : IConsoleApplication
{
    private readonly IConsoleMenu _menu;

    private readonly IAuthenticationApiClient _authenticationApiClient;

    private readonly IConsoleInput _consoleInput;


    public ConsoleApplication(
        IConsoleMenu menu,
        IConsoleInput consoleInput,
        IAuthenticationApiClient authenticationApiClient)
    {
        _menu = menu;
        _consoleInput = consoleInput;
        _authenticationApiClient = authenticationApiClient;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            var option = await _menu.ShowAsync();

            switch (option)
            {
                case MenuOption.Login:
                    await LoginAsync();
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

        Console.WriteLine();
        Console.WriteLine("Login successful!");
        Console.WriteLine();

        Console.WriteLine($"Username: {response.Username}");
        Console.WriteLine($"Email: {response.Email}");
        Console.WriteLine();

        Console.WriteLine("Token:");
        Console.WriteLine(response.Token);
    }
}