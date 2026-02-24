import { type FormEvent, useEffect, useState } from 'react'
import {
  apiClient,
  type CategoryDto,
  type PersonDto,
  type TransactionDto,
  type TransactionInputDto,
  type TransactionType,
} from '../../services/ApiClient'
import { useAuth } from '../../context/AuthContext'

const TRANSACTION_LABEL: Record<TransactionType, string> = {
  Expense: 'Despesa',
  Income: 'Receita',
}

// Tela de cadastro e listagem de transações.
// A API garante as regras: menores de 18 só podem ter despesas e a categoria deve aceitar o tipo escolhido.
export function TransactionsPage() {
  const { role, personId: currentPersonId } = useAuth()
  const [transactions, setTransactions] = useState<TransactionDto[]>([])
  const [people, setPeople] = useState<PersonDto[]>([])
  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [form, setForm] = useState<TransactionInputDto>({
    description: '',
    amount: 0,
    type: 'Expense',
    categoryId: 0,
    personId: 0,
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function loadAll() {
    setLoading(true)
    setError(null)
    try {
      if (role === 'User') {
        const [txs, categoriesData] = await Promise.all([
          apiClient.getTransactions(),
          apiClient.getCategories(),
        ])
        setTransactions(txs)
        setCategories(categoriesData)
        setPeople([])
        setForm((prev) => ({
          ...prev,
          personId: currentPersonId ?? 0,
          categoryId: prev.categoryId || categoriesData[0]?.id || 0,
        }))
      } else {
        const [txs, peopleData, categoriesData] = await Promise.all([
          apiClient.getTransactions(),
          apiClient.getPeople(),
          apiClient.getCategories(),
        ])
        setTransactions(txs)
        setPeople(peopleData)
        setCategories(categoriesData)
        setForm((prev) => ({
          ...prev,
          personId: prev.personId || peopleData[0]?.id || 0,
          categoryId: prev.categoryId || categoriesData[0]?.id || 0,
        }))
      }
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void loadAll()
  }, [])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)

    try {
      if (!form.description.trim()) {
        setError('Descrição é obrigatória.')
        return
      }

      if (form.amount <= 0) {
        setError('Valor deve ser positivo.')
        return
      }

      if ((!form.personId && role === 'Admin') || !form.categoryId) {
        setError('Selecione uma pessoa (se aplicável) e uma categoria.')
        return
      }

      const payload: TransactionInputDto =
        role === 'User' && currentPersonId
          ? { ...form, personId: currentPersonId }
          : form

      await apiClient.createTransaction(payload)
      setForm((prev) => ({
        ...prev,
        description: '',
        amount: 0,
      }))
      await loadAll()
    } catch (e) {
      setError((e as Error).message)
    }
  }

  return (
    <section>
      <header className="page-header">
        <h2>Transações</h2>
        <p>
          Cadastro de receitas e despesas. As regras de menor de idade e de finalidade de categoria são aplicadas na
          API.
        </p>
      </header>

      <form className="card form" onSubmit={handleSubmit}>
        <div className="form-row">
          <label>
            Descrição
            <input
              type="text"
              value={form.description}
              maxLength={400}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
            />
          </label>
        </div>
        <div className="form-row">
          <label>
            Valor
            <input
              type="number"
              min={0}
              step="0.01"
              value={form.amount}
              onChange={(e) => setForm((f) => ({ ...f, amount: Number(e.target.value) }))}
            />
          </label>
          <label>
            Tipo
            <select
              value={form.type}
              onChange={(e) => setForm((f) => ({ ...f, type: e.target.value as TransactionType }))}
            >
              <option value="Expense">Despesa</option>
              <option value="Income">Receita</option>
            </select>
          </label>
        </div>
        <div className="form-row">
          {role === 'Admin' && (
            <label>
              Pessoa
              <select
                value={form.personId}
                onChange={(e) => setForm((f) => ({ ...f, personId: Number(e.target.value) }))}
              >
                <option value={0}>Selecione...</option>
                {people.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.name} ({p.age} anos)
                  </option>
                ))}
              </select>
            </label>
          )}
          <label>
            Categoria
            <select
              value={form.categoryId}
              onChange={(e) => setForm((f) => ({ ...f, categoryId: Number(e.target.value) }))}
            >
              <option value={0}>Selecione...</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.description}
                </option>
              ))}
            </select>
          </label>
        </div>
        {error && <p className="error">{error}</p>}
        <button type="submit" className="primary">
          Registrar transação
        </button>
      </form>

      <div className="card">
        <div className="card-header">
          <h3>Histórico de transações</h3>
          {loading && <span>Carregando...</span>}
        </div>
        <div className="list-scroll">
          {transactions.length === 0 ? (
            <p>Nenhuma transação cadastrada ainda.</p>
          ) : (
            <table className="table">
              <thead>
                <tr>
                  <th>Descrição</th>
                  <th>Tipo</th>
                  <th>Valor</th>
                  <th>Pessoa</th>
                  <th>Categoria</th>
                </tr>
              </thead>
              <tbody>
                {transactions.map((t) => (
                  <tr key={t.id}>
                    <td>{t.description}</td>
                    <td>{TRANSACTION_LABEL[t.type]}</td>
                    <td>{t.amount.toFixed(2)}</td>
                    <td>{t.personName}</td>
                    <td>{t.categoryDescription}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </section>
  )
}

