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

    CreateChannel = 13,

    ListChannels = 14,

    SelectChannel = 15,

    RenameChannel = 16,

    DeleteChannel = 17,

    LoadMessages = 18,

    SendMessage = 19,

    UpdateMessage = 20,

    DeleteMessage = 21,

    ConnectSignalR = 22,

    DisconnectSignalR = 23,

    Logout = 24,
}