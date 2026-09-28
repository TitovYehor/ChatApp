import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'

import { register } from '../api/authApi'
import { useAuth } from '../features/auth/useAuth'
import { ApiError } from '../api/ApiError'

import './css/AuthPages.css'

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
        <main className="auth-page">
            <section className="auth-card">
                <div className="auth-card__brand">
                    <div className="auth-card__logo">
                        C
                    </div>

                    <span>Chat App</span>
                </div>

                <div className="auth-card__heading">
                    <h1>Create your account</h1>
                    <p>
                        Join the conversation and connect with your team.
                    </p>
                </div>

                <form
                    className="auth-form"
                    onSubmit={handleSubmit}
                >
                    <div className="auth-form__field">
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
                            placeholder="Choose a username"
                            required
                            maxLength={50}
                            autoComplete="username"
                        />
                    </div>

                    <div className="auth-form__field">
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
                            placeholder="you@example.com"
                            required
                            autoComplete="email"
                        />
                    </div>

                    <div className="auth-form__field">
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
                            placeholder="At least 6 characters"
                            required
                            minLength={6}
                            autoComplete="new-password"
                        />
                    </div>

                    {error && (
                        <p
                            className="auth-form__error"
                            role="alert"
                        >
                            {error}
                        </p>
                    )}

                    <button
                        className="auth-form__submit"
                        type="submit"
                        disabled={isSubmitting}
                    >
                        {isSubmitting
                            ? 'Creating account...'
                            : 'Create account'}
                    </button>
                </form>

                <p className="auth-card__footer">
                    Already have an account?{' '}
                    <Link to="/login">
                        Login
                    </Link>
                </p>
            </section>
        </main>
    )
}

export default RegisterPage