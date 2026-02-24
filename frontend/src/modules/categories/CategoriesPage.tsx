import { type FormEvent, useEffect, useState } from 'react'
import { apiClient, type CategoryDto, type CategoryInputDto, type CategoryPurpose } from '../../services/ApiClient'

const PURPOSE_LABEL: Record<CategoryPurpose, string> = {
  Expense: 'Despesa',
  Income: 'Receita',
  Both: 'Ambas',
}

// Tela de cadastro de categorias.
// Aqui o usuário define se a categoria é usada para despesa, receita ou ambas.
export function CategoriesPage() {
  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [form, setForm] = useState<CategoryInputDto>({ description: '', purpose: 'Expense' })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    setError(null)
    try {
      const data = await apiClient.getCategories()
      setCategories(data)
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void load()
  }, [])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)

    try {
      if (!form.description.trim()) {
        setError('Descrição é obrigatória.')
        return
      }

      await apiClient.createCategory(form)
      setForm({ description: '', purpose: 'Expense' })
      await load()
    } catch (e) {
      setError((e as Error).message)
    }
  }

  return (
    <section>
      <header className="page-header">
        <h2>Categorias</h2>
        <p>
          Cadastre categorias marcando sua finalidade. A API valida se uma categoria de receita/despesa pode ser usada
          pela transação.
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
          <label>
            Finalidade
            <select
              value={form.purpose}
              onChange={(e) => setForm((f) => ({ ...f, purpose: e.target.value as CategoryPurpose }))}
            >
              <option value="Expense">Despesa</option>
              <option value="Income">Receita</option>
              <option value="Both">Ambas</option>
            </select>
          </label>
        </div>
        {error && <p className="error">{error}</p>}
        <button type="submit" className="primary">
          Adicionar categoria
        </button>
      </form>

      <div className="card">
        <div className="card-header">
          <h3>Lista de categorias</h3>
          {loading && <span>Carregando...</span>}
        </div>
        <div className="list-scroll">
          {categories.length === 0 ? (
            <p>Nenhuma categoria cadastrada ainda.</p>
          ) : (
            <table className="table">
              <thead>
                <tr>
                  <th>Descrição</th>
                  <th>Finalidade</th>
                </tr>
              </thead>
              <tbody>
                {categories.map((c) => (
                  <tr key={c.id}>
                    <td>{c.description}</td>
                    <td>{PURPOSE_LABEL[c.purpose]}</td>
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

