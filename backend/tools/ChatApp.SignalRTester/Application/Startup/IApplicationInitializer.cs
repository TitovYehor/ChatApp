namespace ChatApp.SignalRTester.Application.Startup;

public interface IApplicationInitializer
{
    Task InitializeAsync();
}