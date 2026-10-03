import WorkspaceMemberAddForm from './WorkspaceMemberAddForm'
import WorkspaceMemberRoleSelect from './WorkspaceMemberRoleSelect'
import WorkspaceOwnershipTransfer from './WorkspaceOwnershipTransfer'
import WorkspaceLeave from './WorkspaceLeave'

import './css/WorkspaceMembers.css'

import type {
    WorkspaceMemberResponse,
    WorkspaceRole,
} from '../../types/workspaceTypes'

import type {
    OnlineUserResponse,
} from '../../types/presenceTypes'

interface WorkspaceMembersProps {
    members: WorkspaceMemberResponse[]
    onlineUsers: OnlineUserResponse[]
    displayMembers: WorkspaceMemberResponse[]

    searchQuery: string
    searchError: string | null

    currentUserId: string | null

    workspaceName: string
    currentUserRole: WorkspaceRole | null

    canManageMembers: boolean

    isAdding: boolean
    addError: string | null

    isRemoving: boolean
    removingMember: string | null
    removeError: string | null

    isChangingMemberRole: boolean
    changingMemberRole: string | null
    changeMemberRoleError: string | null

    isTransferringOwnership: boolean
    transferringOwnership: string | null
    transferOwnershipError: string | null

    isLeaving: boolean
    leaveError: string | null

    onSearchChange: (
        value: string,
    ) => void

    onAddMember: (
        usernameOrEmail: string,
    ) => Promise<void>

    onRemoveMember: (
        usernameOrEmail: string,
    ) => Promise<void>

    onChangeMemberRole: (
        usernameOrEmail: string,
        role: WorkspaceRole,
    ) => Promise<void>

    onTransferOwnership: (
        usernameOrEmail: string,
    ) => Promise<void>

    onLeaveWorkspace: () => Promise<void>
}

function WorkspaceMembers({
    members,
    onlineUsers,
    displayMembers,

    searchQuery,
    searchError,
    onSearchChange,

    currentUserId,

    workspaceName,
    currentUserRole,

    canManageMembers,

    isAdding,
    addError,

    isRemoving,
    removingMember,
    removeError,

    isChangingMemberRole,
    changingMemberRole,
    changeMemberRoleError,

    isTransferringOwnership,
    transferringOwnership,
    transferOwnershipError,

    isLeaving,
    leaveError,

    onAddMember,
    onRemoveMember,
    onChangeMemberRole,
    onTransferOwnership,
    onLeaveWorkspace,
}: WorkspaceMembersProps) {
    const onlineUserIds =
        new Set(
            onlineUsers.map(
                (user) => user.userId,
            ),
        )

    function getRoleName(
        role: WorkspaceRole,
    ) {
        switch (role) {
            case 1:
                return 'Owner'
            case 2:
                return 'Admin'
            case 3:
                return 'Member'
        }
    }

    async function handleRemoveMember(
        usernameOrEmail: string,
    ) {
        const confirmed =
            window.confirm(
                `Are you sure you want to remove ${usernameOrEmail} from this workspace?`,
            )

        if (!confirmed) {
            return
        }

        await onRemoveMember(
            usernameOrEmail,
        )
    }

    async function handleChangeMemberRole(
        usernameOrEmail: string,
        role: WorkspaceRole,
    ) {
        const confirmed =
            window.confirm(
                `Are you sure you want to change ${usernameOrEmail}'s role to ${getRoleName(role)}?`,
            )

        if (!confirmed) {
            return
        }

        await onChangeMemberRole(
            usernameOrEmail,
            role,
        )
    }

    return (
        <div className="workspace-members">
            <div className="workspace-members__header">
                <h3>
                    Members
                </h3>

                <span className="workspace-members__count">
                    {members.length}
                </span>

                <input
                    type="text"
                    value={searchQuery}
                    onChange={(event) =>
                        onSearchChange(
                            event.target.value,
                        )
                    }
                    placeholder="Search members..."
                    aria-label="Search members"
                />
            </div>

            {searchError ? (
                <p className="workspace-members__empty">
                    {searchError}
                </p>
            ) : displayMembers.length ===
                0 ? (
                <p className="workspace-members__empty">
                    {searchQuery
                        ? 'No matching members'
                        : 'No members'}
                </p>
            ) : (
                <ul className="workspace-members__list">
                    {displayMembers.map(
                        (
                            member,
                        ) => {
                            const isCurrentUser =
                                member.userId ===
                                currentUserId

                            const isOnline =
                                isCurrentUser ||
                                onlineUserIds.has(
                                    member.userId,
                                )

                            const isThisMemberBeingRemoved =
                                removingMember ===
                                member.username

                            const isThisMemberChangingRole =
                                changingMemberRole ===
                                member.username

                            return (
                                <li
                                    className="workspace-members__member"
                                    key={
                                        member.userId
                                    }
                                >
                                    <div className="workspace-members__identity">
                                        <span
                                            className={`workspace-members__status ${
                                                isOnline
                                                    ? 'workspace-members__status--online'
                                                    : 'workspace-members__status--offline'
                                            }`}
                                            title={
                                                isOnline
                                                    ? 'Online'
                                                    : 'Offline'
                                            }
                                        />

                                        <strong className="workspace-members__username">
                                            {
                                                member.username
                                            }
                                        </strong>

                                        {isCurrentUser && (
                                            <span className="workspace-members__you">
                                                You
                                            </span>
                                        )}

                                        <span className="workspace-members__role">
                                            {getRoleName(
                                                member.role,
                                            )}
                                        </span>
                                    </div>

                                    {canManageMembers &&
                                        !isCurrentUser &&
                                        member.role !==
                                        1 && (
                                            <div className="workspace-members__actions">
                                                <WorkspaceMemberRoleSelect
                                                    role={
                                                        member.role
                                                    }
                                                    isChanging={
                                                        isThisMemberChangingRole
                                                    }
                                                    onChange={(
                                                        role,
                                                    ) =>
                                                        handleChangeMemberRole(
                                                            member.username,
                                                            role,
                                                        )
                                                    }
                                                />

                                                <button
                                                    className="workspace-members__remove"
                                                    type="button"
                                                    onClick={() =>
                                                        void handleRemoveMember(
                                                            member.username,
                                                        )
                                                    }
                                                    disabled={
                                                        isRemoving ||
                                                        isChangingMemberRole
                                                    }
                                                >
                                                    {isThisMemberBeingRemoved
                                                        ? 'Removing...'
                                                        : 'Remove'}
                                                </button>
                                            </div>
                                        )}

                                    {isThisMemberBeingRemoved &&
                                        removeError && (
                                            <p className="workspace-members__error">
                                                {
                                                    removeError
                                                }
                                            </p>
                                        )}

                                    {isThisMemberChangingRole &&
                                        changeMemberRoleError && (
                                            <p className="workspace-members__error">
                                                {
                                                    changeMemberRoleError
                                                }
                                            </p>
                                        )}
                                </li>
                            )
                        },
                    )}
                </ul>
            )}

            {canManageMembers && (
                <WorkspaceMemberAddForm
                    isAdding={
                        isAdding
                    }
                    addError={
                        addError
                    }
                    onAdd={
                        onAddMember
                    }
                />
            )}

            {canManageMembers && (
                <WorkspaceOwnershipTransfer
                    members={
                        members
                    }
                    currentUserId={
                        currentUserId
                    }
                    isTransferring={
                        isTransferringOwnership
                    }
                    transferringMember={
                        transferringOwnership
                    }
                    transferError={
                        transferOwnershipError
                    }
                    onTransfer={
                        onTransferOwnership
                    }
                />
            )}

            <WorkspaceLeave
                workspaceName={
                    workspaceName
                }
                currentUserId={
                    currentUserId
                }
                currentUserRole={
                    currentUserRole
                }
                members={
                    members
                }
                isLeaving={
                    isLeaving
                }
                leaveError={
                    leaveError
                }
                onLeave={
                    onLeaveWorkspace
                }
            />
        </div>
    )
}

export default WorkspaceMembers