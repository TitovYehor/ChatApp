using ChatApp.SignalRTester.Application;
using ChatApp.SignalRTester.Application.Realtime;
using ChatApp.SignalRTester.Application.Services;
using ChatApp.SignalRTester.Application.Startup;
using ChatApp.SignalRTester.Application.State;
using ChatApp.SignalRTester.Application.Workflows;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.Clients.Channels;
using ChatApp.SignalRTester.Clients.Messages;
using ChatApp.SignalRTester.Clients.Workspaces;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Session;
using ChatApp.SignalRTester.Session.AuthenticationState;
using ChatApp.SignalRTester.SignalR;
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

builder.Services.AddSingleton<UserSession>();
builder.Services.AddSingleton<RealtimeSession>();

builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

builder.Services.AddSingleton<MessageCache>();

builder.Services.AddSingleton<MessageRealtimeHandler>();

builder.Services.AddSingleton<IApplicationInitializer>(serviceProvider =>
        serviceProvider.GetRequiredService<MessageRealtimeHandler>());

builder.Services.AddSingleton<IAuthenticationApiClient, AuthenticationApiClient>();
builder.Services.AddSingleton<IWorkspaceApiClient, WorkspaceApiClient>();
builder.Services.AddSingleton<IChannelApiClient, ChannelApiClient>();
builder.Services.AddSingleton<ISignalRClient, SignalRClient>();
builder.Services.AddSingleton<IMessageApiClient, MessageApiClient>();

builder.Services.AddSingleton<IConsoleMenu, ConsoleMenu>();

builder.Services.AddSingleton<IConsoleApplication, ConsoleApplication>();

builder.Services.AddSingleton<IConsoleInput, ConsoleInput>();
builder.Services.AddSingleton<IConsoleOutput, ConsoleOutput>();

builder.Services.AddSingleton<AuthenticationWorkflow>();
builder.Services.AddSingleton<WorkspaceWorkflow>();
builder.Services.AddSingleton<ChannelWorkflow>();
builder.Services.AddSingleton<SignalRWorkflow>();
builder.Services.AddSingleton<MessageWorkflow>();

builder.Services.AddSingleton<RealtimeSessionManager>();

using var host = builder.Build();

var initializers = host.Services.GetServices<IApplicationInitializer>();

foreach (var initializer in initializers)
{
    await initializer.InitializeAsync();
}

var application = host.Services.GetRequiredService<IConsoleApplication>();

await application.RunAsync();