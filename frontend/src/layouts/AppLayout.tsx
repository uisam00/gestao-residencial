import { NavLink } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

type AppLayoutProps = {
  Routes: React.ReactNode
}
export function AppLayout({ Routes }: AppLayoutProps) {
  const { username, logout } = useAuth()

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <h1 className="app-title">Gastos Residenciais</h1>
        <nav className="nav">
          <NavLink to="/people" className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}>
            Pessoas
          </NavLink>
          <NavLink to="/categories" className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}>
            Categorias
          </NavLink>
          <NavLink to="/transactions" className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}>
            Transações
          </NavLink>
          <NavLink to="/reports" className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}>
            Relatórios
          </NavLink>
        </nav>
        <div className="sidebar-footer">
          <span className="sidebar-user">{username ?? ''}</span>
          <button type="button" className="nav-link logout-btn" onClick={logout}>
            Sair
          </button>
        </div>
      </aside>

      <main className="content">
        {Routes as React.ReactNode}
      </main>
    </div>
  )
}

