import React, { createContext, useCallback, useContext, useEffect, useState } from 'react'
import { apiClient } from '../services/ApiClient'

const STORAGE_KEY_TOKEN = 'gastos_token'
const STORAGE_KEY_USERNAME = 'gastos_username'
const STORAGE_KEY_ROLE = 'gastos_role'
const STORAGE_KEY_PERSON_ID = 'gastos_person_id'

type AuthContextValue = {
  token: string | null
  username: string | null
  role: 'Admin' | 'User' | null
  personId: number | null
  loading: boolean
  login: (username: string, password: string) => Promise<'Admin' | 'User' | null>
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
  const [role, setRole] = useState<'Admin' | 'User' | null>(() => {
    const saved = localStorage.getItem(STORAGE_KEY_ROLE)
    return saved === 'Admin' || saved === 'User' ? saved : null
  })
  const [personId, setPersonId] = useState<number | null>(() => {
    const raw = localStorage.getItem(STORAGE_KEY_PERSON_ID)
    const n = raw ? Number(raw) : NaN
    return Number.isFinite(n) ? n : null
  })
  const [loading, setLoading] = useState(true)

  const setToken = useCallback((value: string | null) => {
    setTokenState(value)
    apiClient.setToken(value)
    if (value) localStorage.setItem(STORAGE_KEY_TOKEN, value)
    else localStorage.removeItem(STORAGE_KEY_TOKEN)
  }, [])

  useEffect(() => {
    const saved = localStorage.getItem(STORAGE_KEY_TOKEN)
    if (!saved) {
      setLoading(false)
      return
    }

    apiClient.setToken(saved)

    ;(async () => {
      try {
        const me = await apiClient.getMe()
        setUsername(me.personName)
        localStorage.setItem(STORAGE_KEY_USERNAME, me.personName)
        setRole(me.role)
        setPersonId(me.personId)
        localStorage.setItem(STORAGE_KEY_ROLE, me.role)
        localStorage.setItem(STORAGE_KEY_PERSON_ID, me.personId.toString())
      } catch {
        // Token inválido/expirado: limpa sessão
        setTokenState(null)
        apiClient.setToken(null)
        localStorage.removeItem(STORAGE_KEY_TOKEN)
        localStorage.removeItem(STORAGE_KEY_USERNAME)
        localStorage.removeItem(STORAGE_KEY_ROLE)
        localStorage.removeItem(STORAGE_KEY_PERSON_ID)
      } finally {
        setLoading(false)
      }
    })()
  }, [setTokenState])

  const login = useCallback(
    async (user: string, password: string): Promise<'Admin' | 'User' | null> => {
      try {
        const result = await apiClient.login(user, password)
        setToken(result.token)

        const me = await apiClient.getMe()
        setUsername(me.personName)
        localStorage.setItem(STORAGE_KEY_USERNAME, me.personName)
        setRole(me.role)
        setPersonId(me.personId)
        localStorage.setItem(STORAGE_KEY_ROLE, me.role)
        localStorage.setItem(STORAGE_KEY_PERSON_ID, me.personId.toString())

        return me.role
      } catch {
        return null
      }
    },
    [setToken],
  )

  const logout = useCallback(() => {
    setToken(null)
    setUsername(null)
    setRole(null)
    setPersonId(null)
    localStorage.removeItem(STORAGE_KEY_USERNAME)
    localStorage.removeItem(STORAGE_KEY_ROLE)
    localStorage.removeItem(STORAGE_KEY_PERSON_ID)
  }, [setToken])

  const value: AuthContextValue = {
    token,
    username,
    role,
    personId,
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
