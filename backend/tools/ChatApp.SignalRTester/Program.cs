using ChatApp.SignalRTester.Application;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.Clients.Realtime;
using ChatApp.SignalRTester.Clients.Workspaces;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.UI;
using ChatApp.SignalRTester.UI.Input;
using ChatApp.SignalRTester.UI.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.json",
    optional: false,
    reloadOnChange: true);

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection(AppSettings.SectionName));

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IAuthenticationApiClient, AuthenticationApiClient>();

builder.Services.AddSingleton<ISignalRClient, SignalRClient>();

builder.Services.AddSingleton<IWorkspaceApiClient, WorkspaceApiClient>();

builder.Services.AddSingleton<IConsoleMenu, ConsoleMenu>();

builder.Services.AddSingleton<IConsoleApplication, ConsoleApplication>();

builder.Services.AddSingleton<IConsoleInput, ConsoleInput>();
builder.Services.AddSingleton<IConsoleOutput, ConsoleOutput>();

builder.Services.AddSingleton<UserSession>();

using var host = builder.Build();

var application = host.Services.GetRequiredService<IConsoleApplication>();

await application.RunAsync();