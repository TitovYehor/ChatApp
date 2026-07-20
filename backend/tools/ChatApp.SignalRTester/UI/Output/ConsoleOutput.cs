using ChatApp.Contracts.Channels.Responses;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.SignalRTester.UI.Output;

public class ConsoleOutput : IConsoleOutput
{
    public bool ReadConfirmation(string message)
    {
        while (true)
        {
            Console.Write($"{message} (y/n): ");

            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (input)
            {
                case "y":
                case "yes":
                    return true;

                case "n":
                case "no":
                    return false;

                default:
                    Console.WriteLine("Please enter 'y' or 'n'");
                    break;
            }
        }
    }

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

    public void WriteWorkspaceMember(
        WorkspaceMemberResponseDto member)
    {
        Console.WriteLine($"Username : {member.Username}");
        Console.WriteLine($"Email    : {member.Email}");
        Console.WriteLine($"Role     : {member.Role}");
        Console.WriteLine($"Joined   : {member.JoinedAt:u}");
    }

    public void WriteWorkspaceMembers(
        IReadOnlyCollection<WorkspaceMemberResponseDto> members)
    {
        if (members.Count == 0)
        {
            WriteInfo("No members");
            return;
        }

        var index = 1;

        foreach (var member in members)
        {
            Console.WriteLine($"{index}");

            WriteWorkspaceMember(member);

            WriteSeparator();

            index++;
        }
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
        Console.WriteLine($"{message.Username}");

        Console.WriteLine($"Created: {message.CreatedAt:u}");

        if (message.UpdatedAt.HasValue)
        {
            Console.WriteLine($"Updated: {message.UpdatedAt:u}");
        }

        Console.WriteLine();

        Console.WriteLine(message.Content);
    }

    public void WriteMessageList(
        IReadOnlyCollection<MessageResponseDto> messages)
    {
        if (messages.Count == 0)
        {
            WriteInfo("No messages");
            return;
        }

        var index = 1;

        foreach (var message in messages)
        {
            Console.WriteLine($"[{index}]");

            WriteMessage(message);

            WriteSeparator();

            index++;
        }
    }

    public void WriteRealtimeMessageCreated(
        MessageResponseDto message,
        string? channelName)
    {
        WriteSeparator();

        WriteSuccess("New message received");

        WriteInfo($"Channel: {channelName}");

        WriteMessage(message);

        WriteSeparator();
    }

    public void WriteRealtimeMessageUpdated(
        MessageResponseDto message)
    {
        WriteSeparator();

        WriteInfo("Message updated");

        WriteMessage(message);

        WriteSeparator();
    }

    public void WriteRealtimeMessageDeleted(
        Guid messageId)
    {
        WriteSeparator();

        WriteInfo($"Message deleted: {messageId}");

        WriteSeparator();
    }
}