import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'

import { login } from '../api/authApi'
import { useAuth } from '../features/auth/useAuth'
import { ApiError } from '../api/ApiError'

function LoginPage() {
    const navigate = useNavigate()
    const { isAuthenticated, login: authenticate } = useAuth()

    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState<string | null>(null)
    const [isSubmitting, setIsSubmitting] = useState(false)

    if (isAuthenticated) {
        return <Navigate to="/chat" replace />
    }

    async function handleSubmit(
        event: React.SubmitEvent,
    ) {
        event.preventDefault()

        setError(null)
        setIsSubmitting(true)

        try {
            const response = await login({
                email,
                password,
            })

            authenticate(
                response.accessToken,
                response.user,
            )

            navigate('/chat', { replace: true })
        } catch (error) {
            if (
                error instanceof ApiError &&
                error.status === 401
            ) {
                setError(
                    'Invalid email or password',
                )
            } else {
                setError(
                    'Unable to connect to the server',
                )
            }
        } finally {
            setIsSubmitting(false)
        }
    }

    return (
        <main>
            <h1>Login</h1>

            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="email">
                        Email
                    </label>

                    <input
                        id="email"
                        type="email"
                        value={email}
                        onChange={(event) =>
                            setEmail(event.target.value)
                        }
                        required
                        autoComplete="email"
                    />
                </div>

                <div>
                    <label htmlFor="password">
                        Password
                    </label>

                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(event) =>
                            setPassword(event.target.value)
                        }
                        required
                        autoComplete="current-password"
                    />
                </div>

                {error && (
                    <p role="alert">
                        {error}
                    </p>
                )}

                <button
                    type="submit"
                    disabled={isSubmitting}
                >
                    {isSubmitting
                        ? 'Logging in...'
                        : 'Login'}
                </button>
            </form>

            <p>
                Don't have an account?{' '}
                <Link to="/register">
                    Register
                </Link>
            </p>
        </main>
    )
}

export default LoginPage