import { Navigate, Route, Routes } from 'react-router-dom'
import { ProtectedRoute } from '../components/ProtectedRoute'
import { LoginPage } from '../modules/auth/LoginPage'
import { AppLayout } from '../layouts/AppLayout'
import { PeoplePage } from '../modules/people/PeoplePage'
import { CategoriesPage } from '../modules/categories/CategoriesPage'
import { TransactionsPage } from '../modules/transactions/TransactionsPage'
import { ReportsPage } from '../modules/reports/ReportsPage'

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="*"
        element={
          <ProtectedRoute>
            <AppLayout Routes={<Routes>
              <Route path="/people" element={<PeoplePage />} />
              <Route path="/categories" element={<CategoriesPage />} />
              <Route path="/transactions" element={<TransactionsPage />} />
              <Route path="/reports" element={<ReportsPage />} />
              <Route path="*" element={<Navigate to="/people" replace />} />
            </Routes>} />
          </ProtectedRoute>
        }
      />
    </Routes>
  )
}

