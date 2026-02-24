import React from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

type Props = { children: React.ReactNode }

// Proteção de rotas para garantir que apenas usuários autenticados possam acessar as páginas.
export function ProtectedRoute({ children }: Props) {
  const { isAuthenticated, loading } = useAuth()
  const location = useLocation()

  if (loading) {
    return (
      <div className="content" style={{ justifyContent: 'center', alignItems: 'center' }}>
        <p>Carregando…</p>
      </div>
    )
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }

  return <>{children}</>
}
