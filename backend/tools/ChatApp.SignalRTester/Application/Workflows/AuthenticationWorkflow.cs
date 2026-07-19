using ChatApp.Contracts.Authentication.Requests;
using ChatApp.SignalRTester.Application.Services;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;

namespace ChatApp.SignalRTester.Application.Workflows;

public class AuthenticationWorkflow
{
    private readonly IAuthenticationApiClient _authenticationApiClient;

    private readonly UserSession _userSession;

    private readonly RealtimeSessionManager _realtimeSessionManager;

    private readonly MessageCache _messageCache;

    private readonly IConsoleInput _consoleInput;

    private readonly IConsoleOutput _consoleOutput;

    public AuthenticationWorkflow(
        IAuthenticationApiClient authenticationApiClient,
        UserSession userSession,
        RealtimeSessionManager realtimeSessionManager,
        MessageCache messageCache,
        IConsoleInput consoleInput,
        IConsoleOutput consoleOutput)
    { 
        _authenticationApiClient = authenticationApiClient;
        _userSession = userSession;
        _realtimeSessionManager = realtimeSessionManager;
        _messageCache = messageCache;
        _consoleInput = consoleInput;
        _consoleOutput = consoleOutput;
    }

    public async Task RegisterAsync()
    {
        _consoleOutput.WriteHeader("Register");

        var username = _consoleInput.ReadRequiredString("Username");

        var email = _consoleInput.ReadRequiredString("Email");

        var password = _consoleInput.ReadRequiredString("Password");

        var request = new RegisterRequestDto
        {
            Username = username,
            Email = email,
            Password = password
        };

        var result = await _authenticationApiClient.RegisterAsync(
            request);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var response = result.Data!;

        _userSession.SignIn(response);

        _consoleOutput.WriteSuccess("Registration successful");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo($"Welcome {_userSession.Username}!");

        _consoleOutput.WriteInfo($"Email: {_userSession.Email}");
    }

    public async Task LoginAsync()
    {
        _consoleOutput.WriteHeader("Login");

        var email = _consoleInput.ReadRequiredString("Email");

        var password = _consoleInput.ReadRequiredString("Password");

        var request = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        var result = await _authenticationApiClient.LoginAsync(request);

        _consoleOutput.WriteSeparator();

        if (!result.IsSuccess)
        {
            _consoleOutput.WriteError(result.ErrorMessage!);
            return;
        }

        var response = result.Data!;

        _userSession.SignIn(response);

        _consoleOutput.WriteSuccess("Login successful");

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteInfo($"Welcome {_userSession.Username}!");
        _consoleOutput.WriteInfo($"Email: {_userSession.Email}");
    }

    public async Task LogoutAsync()
    {
        await _realtimeSessionManager.DisconnectAsync();

        _messageCache.Clear();

        _userSession.SignOut();

        _consoleOutput.WriteSeparator();

        _consoleOutput.WriteSuccess("Logged out successfully");
    }
}