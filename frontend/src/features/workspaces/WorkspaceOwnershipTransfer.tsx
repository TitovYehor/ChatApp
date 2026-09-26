import {
    useState,
} from 'react'

import './css/WorkspaceOwnershipTransfer.css'

import type {
    WorkspaceMemberResponse,
} from '../../types/workspaceTypes'

interface WorkspaceOwnershipTransferProps {
    members: WorkspaceMemberResponse[]
    currentUserId: string | null

    isTransferring: boolean
    transferringMember: string | null
    transferError: string | null

    onTransfer: (
        usernameOrEmail: string,
    ) => Promise<void>
}

function WorkspaceOwnershipTransfer({
    members,
    currentUserId,
    isTransferring,
    transferringMember,
    transferError,
    onTransfer,
}: WorkspaceOwnershipTransferProps) {
    const [
        selectedUsername,
        setSelectedUsername,
    ] = useState('')

    async function handleSubmit(
        event: React.FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault()

        if (!selectedUsername) {
            return
        }

        const member =
            members.find(
                (currentMember) =>
                    currentMember.username ===
                    selectedUsername,
            )

        if (!member) {
            return
        }

        const confirmed =
            window.confirm(
                `Are you sure you want to transfer workspace ownership to ${member.username}? You will become an Admin.`,
            )

        if (!confirmed) {
            return
        }

        await onTransfer(
            member.username,
        )

        setSelectedUsername('')
    }

    const eligibleMembers =
        members.filter(
            (member) =>
                member.userId !==
                currentUserId &&
                member.role !== 1,
        )

    return (
        <form
            className="workspace-ownership-transfer"
            onSubmit={
                handleSubmit
            }
        >
            <h4 className="workspace-ownership-transfer__title">
                Transfer ownership
            </h4>

            <select
                className="workspace-ownership-transfer__select"
                value={
                    selectedUsername
                }
                onChange={(
                    event,
                ) =>
                    setSelectedUsername(
                        event.target.value,
                    )
                }
                disabled={
                    isTransferring
                }
            >
                <option value="">
                    Select a member...
                </option>

                {eligibleMembers.map(
                    (
                        member,
                    ) => (
                        <option
                            key={
                                member.userId
                            }
                            value={
                                member.username
                            }
                        >
                            {
                                member.username
                            }
                        </option>
                    ),
                )}
            </select>

            <button
                className="workspace-ownership-transfer__button"
                type="submit"
                disabled={
                    isTransferring ||
                    !selectedUsername
                }
            >
                {isTransferring
                    ? 'Transferring...'
                    : 'Transfer ownership'}
            </button>

            {transferringMember &&
                transferError && (
                    <p className="workspace-ownership-transfer__error">
                        {
                            transferError
                        }
                    </p>
                )}
        </form>
    )
}

export default WorkspaceOwnershipTransfer