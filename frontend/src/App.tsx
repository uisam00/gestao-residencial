import { AuthProvider } from './context/AuthContext'
import { AppRoutes } from './routes/router'

export function App() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  )
}

