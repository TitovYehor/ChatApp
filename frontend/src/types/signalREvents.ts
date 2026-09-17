export const SignalREvents = {
    MessageCreated: 'MessageCreated',
    MessageUpdated: 'MessageUpdated',
    MessageDeleted: 'MessageDeleted',

    UserPresenceChanged: 'UserPresenceChanged',
    OnlineUsersSnapshot: 'OnlineUsersSnapshot',
    UserTyping: 'UserTyping',

    WorkspaceDeleted: 'WorkspaceDeleted',
    WorkspaceUpdated: 'WorkspaceUpdated',
    WorkspaceMemberAdded: 'WorkspaceMemberAdded',
    WorkspaceMemberRemoved: 'WorkspaceMemberRemoved',

    ChannelCreated: 'ChannelCreated',
    ChannelUpdated: 'ChannelUpdated',
    ChannelDeleted: 'ChannelDeleted',
} as const