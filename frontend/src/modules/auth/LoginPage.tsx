import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'

export function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError('')
    setSubmitting(true)
    try {
      const role = await login(username.trim(), password)
      if (role === 'Admin') {
        navigate('/people', { replace: true })
      } else if (role === 'User') {
        navigate('/reports', { replace: true })
      } else {
        setError('Usuário ou senha inválidos.')
      }
    } catch {
      setError('Erro ao conectar. Tente novamente.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="login-page">
      <div className="login-card card">
        <h1 className="app-title">Gastos Residenciais</h1>
        <p className="login-subtitle">Entre com sua conta</p>
        <form onSubmit={handleSubmit} className="form">
          <label>
            Usuário
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              autoComplete="username"
              required
              autoFocus
            />
          </label>
          <label>
            Senha
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
              required
            />
          </label>
          {error && <p className="error">{error}</p>}
          <button type="submit" className="primary" disabled={submitting}>
            {submitting ? 'Entrando…' : 'Entrar'}
          </button>
        </form>
        </div>
    </div>
  )
}
