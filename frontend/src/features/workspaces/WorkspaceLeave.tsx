import type {
    WorkspaceMemberResponse,
    WorkspaceRole,
} from '../../types/workspaceTypes'

import './css/WorkspaceLeave.css'

interface WorkspaceLeaveProps {
    workspaceName: string
    currentUserId: string | null
    currentUserRole: WorkspaceRole | null
    members: WorkspaceMemberResponse[]

    isLeaving: boolean
    leaveError: string | null

    onLeave: () => Promise<void>
}

function WorkspaceLeave({
    workspaceName,
    currentUserId,
    currentUserRole,
    members,
    isLeaving,
    leaveError,
    onLeave,
}: WorkspaceLeaveProps) {
    const isOwner = currentUserRole === 1

    const isOnlyMember =
        members.length === 1 &&
        members[0]?.userId ===
        currentUserId

    if (isOwner) {
        return (
            <div className="workspace-leave">
                <h4 className="workspace-leave__title">
                    Leave workspace
                </h4>

                <p className="workspace-leave__info">
                    You are the owner of
                    this workspace. Transfer
                    ownership before leaving.
                </p>
            </div>
        )
    }

    if (isOnlyMember) {
        return (
            <div className="workspace-leave">
                <h4 className="workspace-leave__title">
                    Leave workspace
                </h4>

                <p className="workspace-leave__info">
                    You cannot leave a
                    workspace when you are
                    the only member.
                </p>
            </div>
        )
    }

    async function handleLeave() {
        const confirmed =
            window.confirm(
                `Leave workspace "${workspaceName}"?`,
            )

        if (!confirmed) {
            return
        }

        await onLeave()
    }

    return (
        <div className="workspace-leave">
            <h4 className="workspace-leave__title">
                Leave workspace
            </h4>

            <button
                className="workspace-leave__button"
                type="button"
                onClick={() => {
                    void handleLeave()
                }}
                disabled={
                    isLeaving
                }
            >
                {isLeaving
                    ? 'Leaving...'
                    : 'Leave workspace'}
            </button>

            {leaveError && (
                <p className="workspace-leave__error">
                    {leaveError}
                </p>
            )}
        </div>
    )
}

export default WorkspaceLeave