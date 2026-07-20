using ChatApp.Contracts.Channels.Responses;
using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.SignalRTester.UI.Output;

public interface IConsoleOutput
{
    bool ReadConfirmation(string message);

    void WriteHeader(string title);

    void WriteInfo(string message);

    void WriteSuccess(string message);

    void WriteError(string message);

    void WriteSeparator();

    void WriteWorkspace(WorkspaceResponseDto workspace);

    void WriteWorkspaces(IReadOnlyCollection<WorkspaceResponseDto> workspaces);

    void WriteWorkspaceSelection(IReadOnlyList<WorkspaceResponseDto> workspaces);

    void WriteWorkspaceMember(WorkspaceMemberResponseDto member);

    void WriteWorkspaceMembers(IReadOnlyCollection<WorkspaceMemberResponseDto> members);

    void WriteChannel(ChannelResponseDto channel);

    void WriteChannelSelection(IReadOnlyList<ChannelResponseDto> channels);

    void WriteMessage(MessageResponseDto message);

    void WriteMessageList(IReadOnlyCollection<MessageResponseDto> messages);

    void WriteRealtimeMessageCreated(
        MessageResponseDto message,
        string? channelName);

    void WriteRealtimeMessageUpdated(
        MessageResponseDto message);

    void WriteRealtimeMessageDeleted(
        Guid messageId);
}