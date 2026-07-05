using ChatApp.Contracts.Channels.Responses;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.SignalRTester.UI.Output;

public class ConsoleOutput : IConsoleOutput
{
    public void WriteHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
        Console.WriteLine();
    }

    public void WriteInfo(string message)
    {
        Console.WriteLine(message);
    }

    public void WriteSuccess(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine(message);

        Console.ForegroundColor = originalColor;
    }

    public void WriteError(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine(message);

        Console.ForegroundColor = originalColor;
    }

    public void WriteSeparator()
    {
        Console.WriteLine(new string('-', 40));
    }

    public void WriteWorkspace(
        WorkspaceResponseDto workspace)
    {
        Console.WriteLine($"Id: {workspace.Id}");
        Console.WriteLine($"Name: {workspace.Name}");
        Console.WriteLine($"Description: {workspace.Description}");
    }

    public void WriteWorkspaces(
        IReadOnlyCollection<WorkspaceResponseDto> workspaces)
    {
        var index = 1;

        foreach (var workspace in workspaces)
        {
            Console.WriteLine($"{index}");

            WriteWorkspace(workspace);

            Console.WriteLine();

            index++;
        }
    }

    public void WriteWorkspaceSelection(
        IReadOnlyList<WorkspaceResponseDto> workspaces)
    {
        for (var i = 0; i < workspaces.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {workspaces[i].Name}");
        }

        Console.WriteLine();
    }

    public void WriteChannel(
        ChannelResponseDto channel)
    {
        Console.WriteLine($"Id: {channel.Id}");

        Console.WriteLine($"Name: {channel.Name}");

        Console.WriteLine($"Created: {channel.CreatedAt:u}");
    }

    public void WriteChannelSelection(
        IReadOnlyList<ChannelResponseDto> channels)
    {
        for (var i = 0; i < channels.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {channels[i].Name}");
        }

        Console.WriteLine();
    }

    public void WriteMessage(
        MessageResponseDto message)
    {
        Console.WriteLine($"Id:        {message.Id}");
        Console.WriteLine($"Author:    {message.Username}");
        Console.WriteLine($"Content:   {message.Content}");
        Console.WriteLine($"Created:   {message.CreatedAt:u}");

        if (message.UpdatedAt.HasValue)
        {
            Console.WriteLine($"Updated:   {message.UpdatedAt:u}");
        }
    }

    public void WriteMessageList(
        IReadOnlyList<MessageResponseDto> messages)
    {
        if (messages.Count == 0)
        {
            WriteInfo("No messages");
            return;
        }

        for (var i = 0; i < messages.Count; i++)
        {
            Console.WriteLine($"[{i + 1}]");

            WriteMessage(messages[i]);

            WriteSeparator();
        }
    }
}