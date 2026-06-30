namespace ChatApp.SignalRTester.UI.Input;

public class ConsoleInput : IConsoleInput
{
    public string ReadRequiredString(
        string prompt)
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

    public string ReadOptionalString(
        string prompt)
    {
        Console.Write($"{prompt}: ");

        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    public int ReadInt(
        string prompt,
        int min,
        int max)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out var value))
            {
                Console.WriteLine("Please enter a valid number");
                Console.WriteLine();
                continue;
            }

            if (value < min || value > max)
            {
                Console.WriteLine($"Please enter a number between {min} and {max}");

                Console.WriteLine();
                continue;
            }

            return value;
        }
    }

    public Guid ReadGuid(
        string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");

            var input = Console.ReadLine();

            if (Guid.TryParse(input, out var guid))
            {
                return guid;
            }

            Console.WriteLine("Invalid GUID");
            Console.WriteLine();
        }
    }

    public bool ReadConfirmation(
        string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");

            var input =
                Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (input)
            {
                case "y":
                case "yes":
                    return true;

                case "n":
                case "no":
                    return false;

                default:
                    Console.WriteLine("Please enter y or n");
                    Console.WriteLine();
                    break;
            }
        }
    }
}