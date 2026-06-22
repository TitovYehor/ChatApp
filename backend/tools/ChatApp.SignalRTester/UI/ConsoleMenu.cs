using ChatApp.Contracts.Authentication.Requests;
using ChatApp.SignalRTester.Clients.Authentication;

namespace ChatApp.SignalRTester.UI;

public class ConsoleMenu : IConsoleMenu
{
    private readonly IAuthenticationApiClient
        _authenticationApiClient;

    public ConsoleMenu(
        IAuthenticationApiClient authenticationApiClient)
    {
        _authenticationApiClient = authenticationApiClient;
    }

    public async Task RunAsync()
    {
        Console.Clear();

        Console.WriteLine("==================================");
        Console.WriteLine("      ChatApp SignalR Tester      ");
        Console.WriteLine("==================================");
        Console.WriteLine();

        Console.Write("Email: ");
        var email = Console.ReadLine();

        Console.Write("Password: ");
        var password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password is required");
            return;
        }

        var request = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        var response = await _authenticationApiClient.LoginAsync(request);

        if (response == null)
        {
            Console.WriteLine();
            Console.WriteLine("Login failed");

            return;
        }

        Console.WriteLine();
        Console.WriteLine("Login successful");
        Console.WriteLine();

        Console.WriteLine($"Username: {response.Username}");
        Console.WriteLine($"Email: {response.Email}");
        Console.WriteLine();
        Console.WriteLine("JWT:");
        Console.WriteLine(response.Token);

        Console.ReadLine();
        await Task.CompletedTask;
    }
}