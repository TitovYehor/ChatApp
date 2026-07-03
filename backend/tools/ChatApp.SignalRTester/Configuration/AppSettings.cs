namespace ChatApp.SignalRTester.Configuration;

public class AppSettings
{
    public const string SectionName = "AppSettings";

    public string ApiBaseUrl { get; init; } = string.Empty;

    public string HubUrl { get; set; } = string.Empty;
}