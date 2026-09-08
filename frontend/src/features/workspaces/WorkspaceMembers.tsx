import WorkspaceMemberAddForm from './WorkspaceMemberAddForm'
import WorkspaceMemberRoleSelect from './WorkspaceMemberRoleSelect'
import WorkspaceOwnershipTransfer from './WorkspaceOwnershipTransfer'

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

    currentUserId: string | null

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
}

function WorkspaceMembers({
    members,
    onlineUsers,
    currentUserId,
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
    onAddMember,
    onRemoveMember,
    onChangeMemberRole,
    onTransferOwnership,
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
        <div>
            <h3>
                Members
            </h3>

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

            {members.length ===
                0 ? (
                <p>
                    No members
                </p>
            ) : (
                <ul>
                    {members.map(
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
                                    key={
                                        member.userId
                                    }
                                >
                                    <span>
                                        {isOnline
                                            ? '🟢'
                                            : '⚪'}
                                    </span>{' '}

                                    <strong>
                                        {
                                            member.username
                                        }
                                    </strong>

                                    {isCurrentUser && (
                                        <span>
                                            {' '}
                                            (You)
                                        </span>
                                    )}

                                    <span>
                                        {' '}
                                        —{' '}
                                        {
                                            getRoleName(
                                                member.role,
                                            )
                                        }
                                    </span>

                                    {canManageMembers &&
                                        !isCurrentUser &&
                                        member.role !==
                                        1 && (
                                            <>
                                                {' '}

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

                                                {' '}

                                                <button
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
                                            </>
                                        )}

                                    {isThisMemberBeingRemoved &&
                                        removeError && (
                                            <p>
                                                {
                                                    removeError
                                                }
                                            </p>
                                        )}

                                    {isThisMemberChangingRole &&
                                        changeMemberRoleError && (
                                            <p>
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
        </div>
    )
}

export default WorkspaceMembers