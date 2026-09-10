import type {
    WorkspaceRole,
} from '../types/workspaceTypes'

import ChatLayout from '../layouts/ChatLayout'

import WorkspaceSidebar from '../features/workspaces/WorkspaceSidebar'
import ChannelSidebar from '../features/channels/ChannelSidebar'
import WorkspaceMembers from '../features/workspaces/WorkspaceMembers'

import MessageList from '../features/messages/MessageList'
import MessageComposer from '../features/messages/MessageComposer'
import TypingIndicator from '../features/presence/TypingIndicator'

import { useAuth } from '../features/auth/useAuth'
import { useWorkspaceController } from '../features/workspaces/useWorkspaceController'
import { useChannelController } from '../features/channels/useChannelController'
import { useChatController } from '../features/chat/useChatController'
import { useChatNavigation } from '../features/chat/useChatNavigation'
import { usePresence } from '../features/presence/usePresence'

function ChatPage() {
    const {
        user,
        logout,
    } = useAuth()

    const {
        selectedWorkspaceId,
        selectedChannelId,

        selectWorkspace,
        selectChannel,

        selectCreatedWorkspace,
        selectCreatedChannel,

        clearSelectedChannel,
        clearSelectedWorkspace,
        clearCurrentWorkspace,
    } = useChatNavigation()

    const workspace = useWorkspaceController(
        selectedWorkspaceId,
    )

    const channel = useChannelController(
        selectedWorkspaceId,
    )

    const chat = useChatController(
        selectedChannelId,
        user?.id ?? null,
    )

    const {
        onlineUsers,
    } = usePresence()

    async function handleCreateWorkspace(
        name: string,
        description: string,
    ) {
        const createdWorkspace =
            await workspace.createWorkspaceAndReturn(
                name,
                description,
            )

        selectCreatedWorkspace(
            createdWorkspace.id,
        )
    }

    async function handleDeleteWorkspace(
        workspaceId: string,
    ) {
        await workspace.deleteWorkspaceById(
            workspaceId,
        )

        clearSelectedWorkspace(
            workspaceId,
        )
    }

    async function handleLeaveWorkspace() {
        await workspace.leaveSelectedWorkspace()

        clearCurrentWorkspace()
    }

    async function handleCreateChannel(
        name: string,
    ) {
        const createdChannel =
            await channel.createChannelAndReturn(
                name,
            )

        selectCreatedChannel(
            createdChannel.id,
        )
    }

    async function handleDeleteChannel(
        channelId: string,
    ) {
        await channel.deleteChannelById(
            channelId,
        )

        clearSelectedChannel(
            channelId,
        )
    }

    async function handleAddWorkspaceMember(
        usernameOrEmail: string,
    ) {
        await workspace.addMember(
            usernameOrEmail,
        )
    }

    async function handleRemoveWorkspaceMember(
        usernameOrEmail: string,
    ) {
        await workspace.removeMember(
            usernameOrEmail,
        )
    }

    async function handleChangeWorkspaceMemberRole(
        usernameOrEmail: string,
        role: WorkspaceRole,
    ) {
        await workspace.changeWorkspaceMemberRole(
            usernameOrEmail,
            role,
        )
    }

    async function handleTransferWorkspaceOwnership(
        usernameOrEmail: string,
    ) {
        await workspace.transferOwnership(
            usernameOrEmail,
        )
    }

    if (workspace.isLoadingWorkspaces) {
        return (
            <div>
                Loading workspaces...
            </div>
        )
    }

    if (workspace.workspacesError) {
        return (
            <div>
                {
                    workspace.workspacesError
                }
            </div>
        )
    }

    return (
        <ChatLayout
            workspaces={
                <WorkspaceSidebar
                    workspaces={
                        workspace.workspaces
                    }
                    selectedWorkspaceId={
                        selectedWorkspaceId
                    }

                    isCreating={
                        workspace.isCreatingWorkspace
                    }
                    createError={
                        workspace.createWorkspaceError
                    }

                    updatingWorkspaceId={
                        workspace.updatingWorkspaceId
                    }
                    updateWorkspaceError={
                        workspace.updateWorkspaceError
                    }
                    updateErrorWorkspaceId={
                        workspace.updateErrorWorkspaceId
                    }

                    deletingWorkspaceId={
                        workspace.deletingWorkspaceId
                    }
                    deleteWorkspaceError={
                        workspace.deleteWorkspaceError
                    }
                    deleteErrorWorkspaceId={
                        workspace.deleteErrorWorkspaceId
                    }

                    onSelectWorkspace={
                        selectWorkspace
                    }
                    onCreateWorkspace={
                        handleCreateWorkspace
                    }
                    onUpdateWorkspace={
                        workspace.updateWorkspaceDetails
                    }
                    onDeleteWorkspace={
                        handleDeleteWorkspace
                    }
                />
            }

            channels={
                selectedWorkspaceId === null ? (
                    <p>
                        Select a workspace
                    </p>
                ) : workspace.isLoadingMembers ||
                    channel.isLoadingChannels ? (
                    <>
                        {channel.isLoadingChannels && (
                            <p>
                                Loading channels...
                            </p>
                        )}

                        {workspace.isLoadingMembers && (
                            <p>
                                Loading members...
                            </p>
                        )}
                    </>
                ) : channel.channelsError ? (
                    <p>
                        {
                            channel.channelsError
                        }
                    </p>
                ) : workspace.membersError ? (
                    <p>
                        {
                            workspace.membersError
                        }
                    </p>
                ) : (
                    <>
                        <ChannelSidebar
                            channels={
                                channel.channels
                            }
                            selectedChannelId={
                                selectedChannelId
                            }
                            canManageChannels={
                                workspace.canManageChannels
                            }

                            isCreating={
                                channel.isCreating
                            }
                            createError={
                                channel.createError
                            }

                            updatingChannelId={
                                channel.updatingChannelId
                            }
                            updateChannelError={
                                channel.updateChannelError
                            }
                            updateErrorChannelId={
                                channel.updateErrorChannelId
                            }

                            deletingChannelId={
                                channel.deletingChannelId
                            }
                            deleteChannelError={
                                channel.deleteChannelError
                            }
                            deleteErrorChannelId={
                                channel.deleteErrorChannelId
                            }

                            onSelectChannel={
                                selectChannel
                            }
                            onCreateChannel={
                                handleCreateChannel
                            }
                            onUpdateChannel={
                                channel.updateChannelDetails
                            }
                            onDeleteChannel={
                                handleDeleteChannel
                            }
                        />

                        <WorkspaceMembers
                            members={
                                workspace.members
                            }
                            onlineUsers={
                                onlineUsers
                            }
                            currentUserId={
                                user?.id ?? null
                            }

                            workspaceName={
                                workspace.selectedWorkspace?.name ??
                                ''
                            }
                            currentUserRole={
                                workspace.selectedWorkspace?.currentUserRole ??
                                null
                            }

                            canManageMembers={
                                workspace.canManageMembers
                            }

                            isAdding={
                                workspace.isAddingMember
                            }
                            addError={
                                workspace.addMemberError
                            }

                            isRemoving={
                                workspace.isRemovingMember
                            }
                            removingMember={
                                workspace.removingMember
                            }
                            removeError={
                                workspace.removeMemberError
                            }

                            isChangingMemberRole={
                                workspace.isChangingMemberRole
                            }
                            changingMemberRole={
                                workspace.changingMemberRole
                            }
                            changeMemberRoleError={
                                workspace.changeMemberRoleError
                            }

                            isTransferringOwnership={
                                workspace.isTransferringOwnership
                            }
                            transferringOwnership={
                                workspace.transferringOwnership
                            }
                            transferOwnershipError={
                                workspace.transferOwnershipError
                            }

                            isLeaving={
                                workspace.isLeaving &&
                                workspace.leavingWorkspaceId ===
                                selectedWorkspaceId
                            }
                            leaveError={
                                workspace.leavingWorkspaceId ===
                                    selectedWorkspaceId
                                    ? workspace.leaveWorkspaceError
                                    : null
                            }

                            onAddMember={
                                handleAddWorkspaceMember
                            }
                            onRemoveMember={
                                handleRemoveWorkspaceMember
                            }
                            onChangeMemberRole={
                                handleChangeWorkspaceMemberRole
                            }
                            onTransferOwnership={
                                handleTransferWorkspaceOwnership
                            }
                            onLeaveWorkspace={
                                handleLeaveWorkspace
                            }
                        />
                    </>
                )
            }
        >
            <div>
                <p>
                    Logged in as{' '}
                    {
                        user?.username
                    }
                </p>

                <button
                    type="button"
                    onClick={
                        logout
                    }
                >
                    Logout
                </button>
            </div>

            {selectedChannelId === null ? (
                <div>
                    <h1>
                        Chat
                    </h1>

                    <p>
                        Select a channel to
                        start chatting
                    </p>
                </div>
            ) : (
                <section>
                    <h2>
                        Messages
                    </h2>

                    {chat.isMessagesLoading ? (
                        <p>
                            Loading
                            messages...
                        </p>
                    ) : chat.messagesError ? (
                        <p>
                            {
                                chat.messagesError
                            }
                        </p>
                    ) : chat.messages.length ===
                        0 ? (
                        <p>
                            No messages yet
                        </p>
                    ) : (
                        <MessageList
                            messages={
                                chat.messages
                            }
                            currentUserId={
                                user?.id ??
                                null
                            }
                            canManageMessages={
                                workspace.canManageMessages
                            }

                            updatingMessageId={
                                chat.updatingMessageId
                            }
                            deletingMessageId={
                                chat.deletingMessageId
                            }

                            updateError={
                                chat.updateError
                            }
                            updateErrorMessageId={
                                chat.updateErrorMessageId
                            }

                            deleteError={
                                chat.deleteError
                            }
                            deleteErrorMessageId={
                                chat.deleteErrorMessageId
                            }

                            onUpdate={
                                chat.updateChatMessage
                            }
                            onDelete={
                                chat.deleteMessage
                            }
                        />
                    )}

                    <TypingIndicator
                        typingUsers={
                            chat.typingUsers
                        }
                    />

                    <MessageComposer
                        isSending={
                            chat.isSending
                        }
                        sendError={
                            chat.sendError
                        }
                        onSend={
                            chat.sendChatMessage
                        }
                        onStartTyping={
                            chat.startTyping
                        }
                        onStopTyping={
                            chat.stopTyping
                        }
                    />
                </section>
            )}
        </ChatLayout>
    )
}

export default ChatPage