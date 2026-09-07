import type {
    WorkspaceRole,
} from '../../types/workspaceTypes'

interface WorkspaceMemberRoleSelectProps {
    role: WorkspaceRole
    isChanging: boolean
    onChange: (
        role: WorkspaceRole,
    ) => Promise<void>
}

function WorkspaceMemberRoleSelect({
    role,
    isChanging,
    onChange,
}: WorkspaceMemberRoleSelectProps) {
    async function handleChange(
        event: React.ChangeEvent<HTMLSelectElement>,
    ) {
        const newRole =
            Number(
                event.target.value,
            ) as WorkspaceRole

        if (
            newRole === role
        ) {
            return
        }

        await onChange(
            newRole,
        )
    }

    return (
        <select
            value={role}
            onChange={
                handleChange
            }
            disabled={
                isChanging
            }
        >
            <option value={2}>
                Admin
            </option>
            <option value={3}>
                Member
            </option>
        </select>
    )
}

export default WorkspaceMemberRoleSelect