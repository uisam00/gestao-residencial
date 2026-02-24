import { useEffect, useState } from 'react'
import { apiClient, type CategoryDto } from '../services/ApiClient'

type Props = {
  value: number | 0
  onChange: (value: number) => void
  allowAllOption?: boolean
  labelAllText?: string
}

export function CategorySelect({ value, onChange, allowAllOption = false, labelAllText = 'Todas' }: Props) {
  const [categories, setCategories] = useState<CategoryDto[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
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

    void load()
  }, [])

  const selected = categories.find((c) => c.id === value)

  return (
    <>
      <div className="select-with-dot">
        <select
          className="select-with-dot-input"
          value={value}
          onChange={(e) => onChange(Number(e.target.value))}
          style={{
            color: selected?.colorHex ?? '#000000',
          }}
        >
          <option style={{color: '#000'}} value={0}>{allowAllOption ? labelAllText : 'Selecione...'}</option>
          {categories.map((c) => (
            <option
              key={c.id}
              value={c.id}
              style={c.colorHex ? { color: c.colorHex } : undefined}
            >
              {c.colorHex ? '● ' : ''}
              {c.description}
            </option>
          ))}
        </select>

      </div>
      {loading && <small>Carregando categorias...</small>}
      {error && <small className="error">{error}</small>}
    </>
  )
}

