export const SignalREvents = {
    MessageCreated: 'MessageCreated',
    MessageUpdated: 'MessageUpdated',
    MessageDeleted: 'MessageDeleted',

    UserPresenceChanged: 'UserPresenceChanged',
    OnlineUsersSnapshot: 'OnlineUsersSnapshot',
    UserTyping: 'UserTyping',
    PrivateUserTyping: 'PrivateUserTyping',

    WorkspaceDeleted: 'WorkspaceDeleted',
    WorkspaceUpdated: 'WorkspaceUpdated',
    WorkspaceMemberAdded: 'WorkspaceMemberAdded',
    WorkspaceMemberRemoved: 'WorkspaceMemberRemoved',
    WorkspaceMemberRoleChanged: 'WorkspaceMemberRoleChanged',
    WorkspaceOwnershipTransferred: 'WorkspaceOwnershipTransferred',
    WorkspaceLeft: 'WorkspaceLeft',

    ChannelCreated: 'ChannelCreated',
    ChannelUpdated: 'ChannelUpdated',
    ChannelDeleted: 'ChannelDeleted',

    FriendshipChanged: 'FriendshipChanged',

    PrivateMessageCreated: 'PrivateMessageCreated',

    NotificationCreated: 'NotificationCreated',
} as const