import { type FormEvent, useEffect, useState } from 'react'
import { apiClient, type PersonDto, type PersonInputDto } from '../../services/ApiClient'

// Tela de manutenção de pessoas.
// Permite listar, criar, editar e excluir pessoas, respeitando validações da API.
export function PeoplePage() {
  const [people, setPeople] = useState<PersonDto[]>([])
  const [form, setForm] = useState<PersonInputDto>({ name: '', age: 0 })
  const [editingId, setEditingId] = useState<number | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function load() {
    setLoading(true)
    setError(null)
    try {
      const data = await apiClient.getPeople()
      setPeople(data)
    } catch (e) {
      setError((e as Error).message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    // Ao abrir a tela, carregamos a lista atual de pessoas.
    void load()
  }, [])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)

    try {
      if (!form.name.trim()) {
        setError('Nome é obrigatório.')
        return
      }

      if (form.age < 0) {
        setError('Idade não pode ser negativa.')
        return
      }

      if (editingId == null) {
        await apiClient.createPerson(form)
      } else {
        await apiClient.updatePerson(editingId, form)
        setEditingId(null)
      }

      setForm({ name: '', age: 0 })
      await load()
    } catch (e) {
      setError((e as Error).message)
    }
  }

  async function handleEdit(person: PersonDto) {
    // Preenche o formulário com os dados atuais para edição.
    setEditingId(person.id)
    setForm({ name: person.name, age: person.age })
  }

  async function handleDelete(id: number) {
    if (!confirm('Tem certeza que deseja remover esta pessoa e todas as suas transações?')) return

    setError(null)
    try {
      await apiClient.deletePerson(id)
      await load()
    } catch (e) {
      setError((e as Error).message)
    }
  }

  return (
    <section>
      <header className="page-header">
        <h2>Pessoas</h2>
        <p>Cadastro de pessoas da residência. Ao excluir, todas as transações da pessoa também são removidas.</p>
      </header>

      <form className="card form" onSubmit={handleSubmit}>
        <div className="form-row">
          <label>
            Nome
            <input
              type="text"
              value={form.name}
              maxLength={200}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            />
          </label>
          <label>
            Idade
            <input
              type="number"
              value={form.age}
              onChange={(e) => setForm((f) => ({ ...f, age: Number(e.target.value) }))}
              min={0}
            />
          </label>
        </div>
        {error && <p className="error">{error}</p>}
        <button type="submit" className="primary">
          {editingId == null ? 'Adicionar pessoa' : 'Salvar alterações'}
        </button>
        {editingId != null && (
          <button
            type="button"
            className="secondary"
            onClick={() => {
              setEditingId(null)
              setForm({ name: '', age: 0 })
            }}
          >
            Cancelar edição
          </button>
        )}
      </form>

      <div className="card">
        <div className="card-header">
          <h3>Lista de pessoas</h3>
          {loading && <span>Carregando...</span>}
        </div>
        <div className="list-scroll">
          {people.length === 0 ? (
            <p>Nenhuma pessoa cadastrada ainda.</p>
          ) : (
            <table className="table">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Idade</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {people.map((p) => (
                  <tr key={p.id}>
                    <td>{p.name}</td>
                    <td>{p.age}</td>
                    <td>
                      <button type="button" onClick={() => handleEdit(p)}>
                        Editar
                      </button>
                      <button type="button" onClick={() => handleDelete(p.id)} className="danger">
                        Excluir
                      </button>
                    </td>
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

