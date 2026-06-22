using ChatApp.SignalRTester.Application;
using ChatApp.SignalRTester.Clients.Authentication;
using ChatApp.SignalRTester.Clients.Realtime;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.UI;
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

builder.Services.AddSingleton<IConsoleMenu, ConsoleMenu>();

builder.Services.AddSingleton<IConsoleApplication, ConsoleApplication>();

using var host = builder.Build();

var application = host.Services.GetRequiredService<IConsoleApplication>();

await application.RunAsync();