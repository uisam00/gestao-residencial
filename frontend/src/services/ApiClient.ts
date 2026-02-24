// Camada simples de acesso HTTP usada pelas páginas para conversar com a API .NET.
// Mantemos toda a lógica de chamadas em um único lugar para facilitar manutenção e troca de baseURL.

const API_BASE_URL =
  import.meta.env.VITE_API_URL?.toString().replace(/\/+$/, '') || 'http://localhost:5000'

let authToken: string | null = null

export function setToken(token: string | null) {
  authToken = token
}

function authHeaders(): Record<string, string> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' }
  if (authToken) headers['Authorization'] = `Bearer ${authToken}`
  return headers
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    // Tenta extrair mensagem de erro textual retornada pela API.
    const text = await response.text()
    throw new Error(text || `Erro HTTP ${response.status}`)
  }

  if (response.status === 204) {
    // No Content: algumas operações (como DELETE) podem não ter corpo.
    return undefined as unknown as T
  }

  return (await response.json()) as T
}

export type PersonDto = {
  id: number
  name: string
  age: number
  hasUser: boolean
  username?: string | null
  role?: 'Admin' | 'User' | null
}

export type PersonInputDto = {
  name: string
  age: number
  createUser: boolean
  username?: string
  password?: string
  isAdmin: boolean
}

export type CategoryPurpose = 'Expense' | 'Income' | 'Both'

export type CategoryDto = {
  id: number
  description: string
  purpose: CategoryPurpose
  colorHex?: string | null
}

export type CategoryInputDto = {
  description: string
  purpose: CategoryPurpose
  colorHex?: string | null
}

export type TransactionType = 'Expense' | 'Income'

export type TransactionDto = {
  id: number
  description: string
  amount: number
  type: TransactionType
  categoryId: number
  personId: number
  personName: string
  categoryDescription: string
  categoryColorHex?: string | null
}

export type TransactionInputDto = {
  description: string
  amount: number
  type: TransactionType
  categoryId: number
  personId: number
}

export type PersonTotalsDto = {
  id: number
  name: string
  age: number
  totalIncome: number
  totalExpense: number
  balance: number
}

export type PersonTotalsSummaryDto = {
  items: PersonTotalsDto[]
  grandTotalIncome: number
  grandTotalExpense: number
  grandBalance: number
}

export type CategoryTotalsDto = {
  id: number
  description: string
  purpose: CategoryPurpose
  colorHex?: string | null
  totalIncome: number
  totalExpense: number
  balance: number
}

export type CategoryTotalsSummaryDto = {
  items: CategoryTotalsDto[]
  grandTotalIncome: number
  grandTotalExpense: number
  grandBalance: number
}

export type LoginResponse = {
  token: string
  username: string
  personName: string
  role: 'Admin' | 'User'
  personId: number
}

export type MeResponse = {
  personId: number
  personName: string
  age: number
  username: string
  role: 'Admin' | 'User'
}

export const apiClient = {
  // Auth ---------------------------------------------------------------------
  async login(username: string, password: string): Promise<LoginResponse> {
    const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    })
    return handleResponse<LoginResponse>(res)
  },

  setToken,

  async getMe(): Promise<MeResponse> {
    const res = await fetch(`${API_BASE_URL}/api/auth/me`, {
      headers: authHeaders(),
    })
    return handleResponse<MeResponse>(res)
  },

  // Pessoas ------------------------------------------------------------------
  async getPeople(): Promise<PersonDto[]> {
    const res = await fetch(`${API_BASE_URL}/api/people`, { headers: authHeaders() })
    return handleResponse<PersonDto[]>(res)
  },

  async createPerson(input: PersonInputDto): Promise<PersonDto> {
    const res = await fetch(`${API_BASE_URL}/api/people`, {
      method: 'POST',
      headers: authHeaders(),
      body: JSON.stringify(input),
    })
    return handleResponse<PersonDto>(res)
  },

  async updatePerson(id: number, input: PersonInputDto): Promise<PersonDto> {
    const res = await fetch(`${API_BASE_URL}/api/people/${id}`, {
      method: 'PUT',
      headers: authHeaders(),
      body: JSON.stringify(input),
    })
    return handleResponse<PersonDto>(res)
  },

  async deletePerson(id: number): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/api/people/${id}`, {
      method: 'DELETE',
      headers: authHeaders(),
    })
    return handleResponse<void>(res)
  },

  // Categorias ---------------------------------------------------------------
  async getCategories(): Promise<CategoryDto[]> {
    const res = await fetch(`${API_BASE_URL}/api/categories`, { headers: authHeaders() })
    return handleResponse<CategoryDto[]>(res)
  },

  async createCategory(input: CategoryInputDto): Promise<CategoryDto> {
    const res = await fetch(`${API_BASE_URL}/api/categories`, {
      method: 'POST',
      headers: authHeaders(),
      body: JSON.stringify(input),
    })
    return handleResponse<CategoryDto>(res)
  },

  async updateCategory(id: number, input: CategoryInputDto): Promise<CategoryDto> {
    const res = await fetch(`${API_BASE_URL}/api/categories/${id}`, {
      method: 'PUT',
      headers: authHeaders(),
      body: JSON.stringify(input),
    })
    return handleResponse<CategoryDto>(res)
  },

  async deleteCategory(id: number): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/api/categories/${id}`, {
      method: 'DELETE',
      headers: authHeaders(),
    })
    return handleResponse<void>(res)
  },

  // Transações ---------------------------------------------------------------
  async getTransactions(): Promise<TransactionDto[]> {
    const res = await fetch(`${API_BASE_URL}/api/transactions`, { headers: authHeaders() })
    return handleResponse<TransactionDto[]>(res)
  },

  async createTransaction(input: TransactionInputDto): Promise<TransactionDto> {
    const res = await fetch(`${API_BASE_URL}/api/transactions`, {
      method: 'POST',
      headers: authHeaders(),
      body: JSON.stringify(input),
    })
    return handleResponse<TransactionDto>(res)
  },

  async updateTransaction(id: number, input: TransactionInputDto): Promise<TransactionDto> {
    const res = await fetch(`${API_BASE_URL}/api/transactions/${id}`, {
      method: 'PUT',
      headers: authHeaders(),
      body: JSON.stringify(input),
    })
    return handleResponse<TransactionDto>(res)
  },

  async deleteTransaction(id: number): Promise<void> {
    const res = await fetch(`${API_BASE_URL}/api/transactions/${id}`, {
      method: 'DELETE',
      headers: authHeaders(),
    })
    return handleResponse<void>(res)
  },

  // Relatórios ---------------------------------------------------------------
  async getTotalsByPerson(filters?: { personId?: number; categoryId?: number; type?: TransactionType }): Promise<PersonTotalsSummaryDto> {
    const params = new URLSearchParams()
    if (filters?.personId) params.append('personId', String(filters.personId))
    if (filters?.categoryId) params.append('categoryId', String(filters.categoryId))
    if (filters?.type) params.append('type', filters.type)
    const qs = params.toString()
    const url = `${API_BASE_URL}/api/reports/by-person${qs ? `?${qs}` : ''}`
    const res = await fetch(url, { headers: authHeaders() })
    return handleResponse<PersonTotalsSummaryDto>(res)
  },

  async getTotalsByCategory(filters?: { personId?: number; categoryId?: number; type?: TransactionType }): Promise<CategoryTotalsSummaryDto> {
    const params = new URLSearchParams()
    if (filters?.personId) params.append('personId', String(filters.personId))
    if (filters?.categoryId) params.append('categoryId', String(filters.categoryId))
    if (filters?.type) params.append('type', filters.type)
    const qs = params.toString()
    const url = `${API_BASE_URL}/api/reports/by-category${qs ? `?${qs}` : ''}`
    const res = await fetch(url, { headers: authHeaders() })
    return handleResponse<CategoryTotalsSummaryDto>(res)
  },
}

