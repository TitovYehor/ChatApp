namespace ChatApp.SignalRTester.UI.Models;

public enum MenuOption
{
    Exit = 0,

    Register = 1,

    Login = 2,

    CreateWorkspace = 3,

    ListWorkspaces = 4,

    SelectWorkspace = 5,

    EditWorkspace = 6,

    DeleteWorkspace = 7,

    AddWorkspaceMember = 8,

    ListWorkspaceMembers = 9,

    LeaveWorkspace = 10,

    RemoveWorkspaceMember = 11,

    ChangeWorkspaceMemberRole = 12,

    TransferWorkspaceOwnership = 13,

    JoinWorkspace = 14,

    ListOnlineUsers = 15,

    CreateChannel = 16,

    ListChannels = 17,

    SelectChannel = 18,

    RenameChannel = 19,

    DeleteChannel = 20,

    LoadMessages = 21,

    SearchMessages = 22,

    SendMessage = 23,

    UpdateMessage = 24,

    DeleteMessage = 25,

    ConnectSignalR = 26,

    DisconnectSignalR = 27,

    Logout = 28,
}