import type {
    WorkspaceMemberResponse,
} from '../../types/workspaceTypes'

import type {
    OnlineUserResponse,
} from '../../types/presenceTypes'

interface WorkspaceMembersProps {
    members: WorkspaceMemberResponse[]
    onlineUsers: OnlineUserResponse[]
}

function WorkspaceMembers({
    members,
    onlineUsers,
}: WorkspaceMembersProps) {
    const onlineUserIds =
        new Set(
            onlineUsers.map(
                (user) => user.userId,
            ),
        )

    if (members.length === 0) {
        return (
            <div>
                <h3>Members</h3>
                <p>No members</p>
            </div>
        )
    }

    return (
        <div>
            <h3>Members</h3>

            <ul>
                {members.map((member) => {
                    const isOnline =
                        onlineUserIds.has(
                            member.userId,
                        )

                    return (
                        <li key={member.userId}>
                            <span>
                                {isOnline
                                    ? '🟢'
                                    : '⚪'}
                            </span>{' '}
                            <strong>
                                {member.username}
                            </strong>
                        </li>
                    )
                })}
            </ul>
        </div>
    )
}

export default WorkspaceMembers