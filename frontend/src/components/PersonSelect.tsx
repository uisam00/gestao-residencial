import { useEffect, useState } from 'react'
import { apiClient, type PersonDto } from '../services/ApiClient'

type Props = {
  value: number | 0
  onChange: (value: number) => void
  allowAllOption?: boolean
  labelAllText?: string
}

export function PersonSelect({ value, onChange, allowAllOption = false, labelAllText = 'Todas' }: Props) {
  const [people, setPeople] = useState<PersonDto[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
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

    void load()
  }, [])

  return (
    <>
      <select
        value={value}
        onChange={(e) => onChange(Number(e.target.value))}
      >
        <option value={0}>{allowAllOption ? labelAllText : 'Selecione...'}</option>
        {people.map((p) => (
          <option key={p.id} value={p.id}>
            {p.name} ({p.age} anos)
          </option>
        ))}
      </select>
      {loading && <small>Carregando pessoas...</small>}
      {error && <small className="error">{error}</small>}
    </>
  )
}

