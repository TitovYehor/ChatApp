import { useEffect, useState } from 'react'

import type {
    WorkspaceRole,
} from '../types/workspaceTypes'

import ChatHeader from '../layouts/ChatHeader'
import ChatLayout from '../layouts/ChatLayout'
import './css/ChatMessages.css'

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

    const [workspaceSearch, setWorkspaceSearch] =
        useState('')

    const [debouncedWorkspaceSearch, setDebouncedWorkspaceSearch] =
        useState('')

    const [channelSearch, setChannelSearch] =
        useState('')

    const [debouncedChannelSearch, setDebouncedChannelSearch] =
        useState('')

    const [memberSearch, setMemberSearch] =
        useState('')

    const [debouncedMemberSearch, setDebouncedMemberSearch] =
        useState('')

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            setDebouncedWorkspaceSearch(
                workspaceSearch.trim(),
            )
        }, 300)

        return () => {
            clearTimeout(timeoutId)
        }
    }, [workspaceSearch])

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            setDebouncedChannelSearch(
                channelSearch.trim(),
            )
        }, 300)

        return () => {
            clearTimeout(timeoutId)
        }
    }, [channelSearch])

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            setDebouncedMemberSearch(
                memberSearch.trim(),
            )
        }, 300)

        return () => {
            clearTimeout(timeoutId)
        }
    }, [memberSearch])

    const workspace = useWorkspaceController(
        user?.id ?? null,
        selectedWorkspaceId,
        clearSelectedWorkspace,
        debouncedWorkspaceSearch,
        debouncedMemberSearch,
    )

    const channel = useChannelController(
        selectedWorkspaceId,
        selectedChannelId,
        clearSelectedChannel,
        debouncedChannelSearch,
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

    const selectedChannel =
        channel.channels.find(
            (item) => item.id === selectedChannelId,
        ) ?? null

    const displayChannels =
        debouncedChannelSearch
            ? channel.searchResults
            : channel.channels

    const displayMembers =
        debouncedMemberSearch
            ? workspace.memberSearchResults
            : workspace.members

    return (
        <ChatLayout
            header={
                <ChatHeader
                    username={user?.username ?? 'User'}
                    onLogout={logout}
                />
            }
            workspaces={
                <WorkspaceSidebar
                    selectedWorkspaceId={
                        selectedWorkspaceId
                    }

                    searchQuery={workspaceSearch}
                    searchError={workspace.searchError}
                    displayWorkspaces={
                        debouncedWorkspaceSearch
                            ? workspace.searchResults
                            : workspace.workspaces
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
                    onSearchChange={
                        setWorkspaceSearch
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
                    <div className="chat-empty">
                        <p>
                            Select a workspace
                        </p>
                    </div>
                ) : workspace.isLoadingMembers ||
                    channel.isLoadingChannels ? (
                    <div className="chat-empty">
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
                    </div>
                ) : channel.channelsError ? (
                    <p className="chat-messages__error">
                        {
                            channel.channelsError
                        }
                    </p>
                ) : workspace.membersError ? (
                    <p className="chat-messages__error">
                        {
                            workspace.membersError
                        }
                    </p>
                ) : (
                    <div className="chat-page__channels-section">
                        <ChannelSidebar
                            channels={
                                channel.channels
                            }
                            selectedChannelId={
                                selectedChannelId
                            }
                            searchQuery={
                                channelSearch
                            }

                            searchError={
                                channel.searchError
                            }

                            displayChannels={
                                displayChannels
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
                            onSearchChange={
                                setChannelSearch
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
                            displayMembers={
                                displayMembers
                            }

                            searchQuery={
                                memberSearch
                            }

                            searchError={
                                workspace.memberSearchError
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

                            onSearchChange={
                                setMemberSearch
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
                    </div>
                )
            }
        >
            {selectedChannelId === null ? (
                <div className="chat-empty">
                    <div className="chat-empty__icon">#</div>

                    <h1>Chat</h1>

                    <p>
                        Select a channel to start chatting
                    </p>
                </div>
            ) : (
                <section className="chat-messages">
                    <div className="chat-messages__header">
                        <div className="chat-messages__channel-icon">
                            #
                        </div>

                        <div className="chat-messages__channel-info">
                            <h2 className="chat-messages__channel-name">
                                {selectedChannel?.name ?? 'Channel'}
                            </h2>
                        </div>
                    </div>

                    {chat.isMessagesLoading ? (
                        <p className="chat-messages__status">
                            Loading
                            messages...
                        </p>
                    ) : chat.messagesError ? (
                        <p className="chat-messages__error">
                            {
                                chat.messagesError
                            }
                        </p>
                    ) : chat.messages.length ===
                        0 ? (
                        <p className="chat-messages__status">
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