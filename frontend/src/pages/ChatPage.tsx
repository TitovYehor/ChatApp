import { useWorkspaces } from '../features/workspaces/useWorkspaces'

function ChatPage() {
    const {
        workspaces,
        isLoading,
        error,
    } = useWorkspaces()

    if (isLoading) {
        return <div>Loading workspaces...</div>
    }

    if (error) {
        return <div>{error}</div>
    }

    return (
        <main>
            <h1>Chat</h1>

            <h2>Workspaces</h2>

            {workspaces.length === 0 ? (
                <p>No workspaces found</p>
            ) : (
                <ul>
                    {workspaces.map((workspace) => (
                        <li key={workspace.id}>
                            {workspace.name}
                        </li>
                    ))}
                </ul>
            )}
        </main>
    )
}

export default ChatPage