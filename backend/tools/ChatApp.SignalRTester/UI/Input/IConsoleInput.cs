namespace ChatApp.SignalRTester.UI.Input;

public interface IConsoleInput
{
    string ReadRequiredString(string prompt);

    string ReadOptionalString(string prompt);

    int ReadInt(
        string prompt,
        int min,
        int max);

    Guid ReadGuid(string prompt);

    bool ReadConfirmation(string prompt);
}