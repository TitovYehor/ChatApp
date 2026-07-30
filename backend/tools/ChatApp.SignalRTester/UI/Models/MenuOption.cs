namespace ChatApp.SignalRTester.UI.Models;

public enum MenuOption
{
    Exit = 0,

    Register = 1,

    Login = 2,

    CreateWorkspace = 3,

    ListWorkspaces = 4,

    SelectWorkspace = 5,

    AddWorkspaceMember = 6,

    ListWorkspaceMembers = 7,

    LeaveWorkspace = 8,

    RemoveWorkspaceMember = 9,

    ChangeWorkspaceMemberRole = 10,

    TransferWorkspaceOwnership = 11,

    JoinWorkspace = 12,

    ListOnlineUsers = 13,

    CreateChannel = 14,

    ListChannels = 15,

    SelectChannel = 16,

    RenameChannel = 17,

    DeleteChannel = 18,

    LoadMessages = 19,

    SendMessage = 20,

    UpdateMessage = 21,

    DeleteMessage = 22,

    ConnectSignalR = 23,

    DisconnectSignalR = 24,

    Logout = 25,
}