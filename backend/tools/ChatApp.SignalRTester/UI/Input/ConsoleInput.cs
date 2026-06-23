namespace ChatApp.SignalRTester.UI.Input;

public class ConsoleInput : IConsoleInput
{
    public string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");

            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Value is required");
            Console.WriteLine();
        }
    }
}