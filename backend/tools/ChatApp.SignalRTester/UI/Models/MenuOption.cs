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

    LoadMessages = 16,

    SendMessage = 17,

    UpdateMessage = 18,

    DeleteMessage = 19,

    ConnectSignalR = 20,

    DisconnectSignalR = 21,

    Logout = 22,
}