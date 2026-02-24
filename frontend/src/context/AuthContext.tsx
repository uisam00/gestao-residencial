import React, { createContext, useCallback, useContext, useEffect, useState } from 'react'
import { apiClient } from '../services/ApiClient'

const STORAGE_KEY_TOKEN = 'gastos_token'
const STORAGE_KEY_USERNAME = 'gastos_username'

type AuthContextValue = {
  token: string | null
  username: string | null
  loading: boolean
  login: (username: string, password: string) => Promise<boolean>
  logout: () => void
  isAuthenticated: boolean
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setTokenState] = useState<string | null>(() =>
    localStorage.getItem(STORAGE_KEY_TOKEN),
  )
  const [username, setUsername] = useState<string | null>(() =>
    localStorage.getItem(STORAGE_KEY_USERNAME),
  )
  const [loading, setLoading] = useState(true)

  const setToken = useCallback((value: string | null) => {
    setTokenState(value)
    apiClient.setToken(value)
    if (value) localStorage.setItem(STORAGE_KEY_TOKEN, value)
    else localStorage.removeItem(STORAGE_KEY_TOKEN)
  }, [])

  useEffect(() => {
    const saved = localStorage.getItem(STORAGE_KEY_TOKEN)
    if (saved) apiClient.setToken(saved)
    setLoading(false)
  }, [])

  const login = useCallback(
    async (user: string, password: string): Promise<boolean> => {
      try {
        const result = await apiClient.login(user, password)
        setToken(result.token)
        setUsername(result.username)
        localStorage.setItem(STORAGE_KEY_USERNAME, result.username)
        return true
      } catch {
        return false
      }
    },
    [setToken],
  )

  const logout = useCallback(() => {
    setToken(null)
    setUsername(null)
    localStorage.removeItem(STORAGE_KEY_USERNAME)
  }, [setToken])

  const value: AuthContextValue = {
    token,
    username,
    loading,
    login,
    logout,
    isAuthenticated: !!token,
  }

  return React.createElement(AuthContext.Provider, { value }, children)
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
