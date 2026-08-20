import { type FormEvent, useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'

import { register } from '../api/authApi'
import { useAuth } from '../features/auth/AuthContext'
import { ApiError } from '../api/ApiError'

function RegisterPage() {
    const navigate = useNavigate()
    const { isAuthenticated, login: authenticate } = useAuth()

    const [username, setUsername] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState<string | null>(null)
    const [isSubmitting, setIsSubmitting] = useState(false)

    if (isAuthenticated) {
        return <Navigate to="/chat" replace />
    }

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault()

        setError(null)
        setIsSubmitting(true)

        try {
            const response = await register({
                username,
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
                error.status === 409
            ) {
                setError(
                    'That email or username is already in use',
                )
            } else {
                setError(
                    'Unable to create your account',
                )
            }
        } finally {
            setIsSubmitting(false)
        }
    }

    return (
        <main>
            <h1>Register</h1>

            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="username">
                        Username
                    </label>

                    <input
                        id="username"
                        type="text"
                        value={username}
                        onChange={(event) =>
                            setUsername(event.target.value)
                        }
                        required
                        maxLength={50}
                        autoComplete="username"
                    />
                </div>

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
                        minLength={6}
                        autoComplete="new-password"
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
                        ? 'Creating account...'
                        : 'Register'}
                </button>
            </form>

            <p>
                Already have an account?{' '}
                <Link to="/login">
                    Login
                </Link>
            </p>
        </main>
    )
}

export default RegisterPage