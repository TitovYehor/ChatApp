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

    canManageMembers: boolean

    isAdding: boolean
    addError: string | null

    onAddMember: (
        usernameOrEmail: string,
    ) => Promise<void>
}

function WorkspaceMembers({
    members,
    onlineUsers,
    canManageMembers,
    isAdding,
    addError,
    onAddMember,
}: WorkspaceMembersProps) {
    const onlineUserIds =
        new Set(
            onlineUsers.map(
                (user) => user.userId,
            ),
        )

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
                            const isOnline =
                                onlineUserIds.has(
                                    member.userId,
                                )

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