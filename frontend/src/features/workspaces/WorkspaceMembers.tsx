import WorkspaceMemberAddForm from './WorkspaceMemberAddForm'

import type {
    WorkspaceMemberResponse,
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

    onAddMember: (
        usernameOrEmail: string,
    ) => Promise<void>

    onRemoveMember: (
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
    onAddMember,
    onRemoveMember,
}: WorkspaceMembersProps) {
    const onlineUserIds =
        new Set(
            onlineUsers.map(
                (user) => user.userId,
            ),
        )

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

                                    {canManageMembers &&
                                        !isCurrentUser && (
                                            <>
                                                {' '}

                                                <button
                                                    type="button"
                                                    onClick={() =>
                                                        void handleRemoveMember(
                                                            member.username,
                                                        )
                                                    }
                                                    disabled={
                                                        isRemoving
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