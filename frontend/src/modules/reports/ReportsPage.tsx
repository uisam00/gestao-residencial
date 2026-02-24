import { useEffect, useState } from 'react'
import { apiClient, type CategoryTotalsSummaryDto, type PersonTotalsSummaryDto } from '../../services/ApiClient'

// Tela de relatórios consolidados por pessoa e por categoria.
// Apenas leitura: os totais já vêm calculados pela API a partir das transações.
export function ReportsPage() {
  const [personTotals, setPersonTotals] = useState<PersonTotalsSummaryDto | null>(null)
  const [categoryTotals, setCategoryTotals] = useState<CategoryTotalsSummaryDto | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    setError(null)
    try {
      const [byPerson, byCategory] = await Promise.all([
        apiClient.getTotalsByPerson(),
        apiClient.getTotalsByCategory(),
      ])
      setPersonTotals(byPerson)
      setCategoryTotals(byCategory)
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void load()
  }, [])

  return (
    <section>
      <header className="page-header">
        <h2>Relatórios</h2>
        <p>
          Totais de receitas, despesas e saldo por pessoa e por categoria, além dos totais gerais consolidados do
          sistema.
        </p>
      </header>

      {error && <p className="error">{error}</p>}
      {loading && <p>Carregando relatórios...</p>}

      {/* Totais por pessoa */}
      <div className="card">
        <div className="card-header">
          <h3>Totais por pessoa</h3>
        </div>
        {personTotals && personTotals.items.length > 0 ? (
          <>
            <div className="list-scroll">
              <table className="table">
                <thead>
                  <tr>
                    <th>Pessoa</th>
                    <th>Idade</th>
                    <th>Receitas</th>
                    <th>Despesas</th>
                    <th>Saldo</th>
                  </tr>
                </thead>
                <tbody>
                  {personTotals.items.map((p) => (
                    <tr key={p.id}>
                      <td>{p.name}</td>
                      <td>{p.age}</td>
                      <td>{p.totalIncome.toFixed(2)}</td>
                      <td>{p.totalExpense.toFixed(2)}</td>
                      <td className={p.balance >= 0 ? 'positive' : 'negative'}>{p.balance.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="summary-row">
              <span>Total geral receitas: {personTotals.grandTotalIncome.toFixed(2)}</span>
              <span>Total geral despesas: {personTotals.grandTotalExpense.toFixed(2)}</span>
              <span className={personTotals.grandBalance >= 0 ? 'positive' : 'negative'}>
                Saldo líquido geral: {personTotals.grandBalance.toFixed(2)}
              </span>
            </div>
          </>
        ) : (
          <p>Nenhuma transação cadastrada para cálculo de totais por pessoa.</p>
        )}
      </div>

      {/* Totais por categoria */}
      <div className="card">
        <div className="card-header">
          <h3>Totais por categoria</h3>
        </div>
        {categoryTotals && categoryTotals.items.length > 0 ? (
          <>
            <div className="list-scroll">
              <table className="table">
                <thead>
                  <tr>
                    <th>Categoria</th>
                    <th>Finalidade</th>
                    <th>Receitas</th>
                    <th>Despesas</th>
                    <th>Saldo</th>
                  </tr>
                </thead>
                <tbody>
                  {categoryTotals.items.map((c) => (
                    <tr key={c.id}>
                      <td>{c.description}</td>
                      <td>{c.purpose}</td>
                      <td>{c.totalIncome.toFixed(2)}</td>
                      <td>{c.totalExpense.toFixed(2)}</td>
                      <td className={c.balance >= 0 ? 'positive' : 'negative'}>{c.balance.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="summary-row">
              <span>Total geral receitas: {categoryTotals.grandTotalIncome.toFixed(2)}</span>
              <span>Total geral despesas: {categoryTotals.grandTotalExpense.toFixed(2)}</span>
              <span className={categoryTotals.grandBalance >= 0 ? 'positive' : 'negative'}>
                Saldo líquido geral: {categoryTotals.grandBalance.toFixed(2)}
              </span>
            </div>
          </>
        ) : (
          <p>Nenhuma transação cadastrada para cálculo de totais por categoria.</p>
        )}
      </div>
    </section>
  )
}

