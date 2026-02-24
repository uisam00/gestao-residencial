import { type FormEvent, useEffect, useState } from "react";
import {
  apiClient,
  type TransactionDto,
  type TransactionInputDto,
  type TransactionType,
} from "../../services/ApiClient";
import { useAuth } from "../../context/AuthContext";
import { PersonSelect } from "../../components/PersonSelect";
import { CategorySelect } from "../../components/CategorySelect";

const TRANSACTION_LABEL: Record<TransactionType, string> = {
  Expense: "Despesa",
  Income: "Receita",
};

// Tela de cadastro e listagem de transações.
// A API garante as regras: menores de 18 só podem ter despesas e a categoria deve aceitar o tipo escolhido.
export function TransactionsPage() {
  const { role, personId: currentPersonId } = useAuth();
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [form, setForm] = useState<TransactionInputDto>({
    description: "",
    amount: 0,
    type: "Expense",
    categoryId: 0,
    personId: 0,
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function loadAll() {
    setLoading(true);
    setError(null);
    try {
      if (role === "User") {
        const txs = await apiClient.getTransactions();
        setTransactions(txs);
        setForm((prev) => ({
          ...prev,
          personId: currentPersonId ?? 0,
        }));
      } else {
        const txs = await apiClient.getTransactions();
        setTransactions(txs);
      }
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void loadAll();
  }, []);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      if (!form.description.trim()) {
        setError("Descrição é obrigatória.");
        return;
      }

      if (form.amount <= 0) {
        setError("Valor deve ser positivo.");
        return;
      }

      if ((!form.personId && role === "Admin") || !form.categoryId) {
        setError("Selecione uma pessoa (se aplicável) e uma categoria.");
        return;
      }

      const payload: TransactionInputDto =
        role === "User" && currentPersonId
          ? { ...form, personId: currentPersonId }
          : form;

      if (editingId == null) {
        await apiClient.createTransaction(payload);
      } else {
        await apiClient.updateTransaction(editingId, payload);
        setEditingId(null);
      }
      setForm((prev) => ({
        ...prev,
        description: "",
        amount: 0,
      }));
      await loadAll();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  function handleEdit(tx: TransactionDto) {
    setEditingId(tx.id);
    setError(null);
    setForm({
      description: tx.description,
      amount: tx.amount,
      type: tx.type,
      categoryId: tx.categoryId,
      personId: tx.personId,
    });
  }

  async function handleDelete(id: number) {
    if (!confirm("Tem certeza que deseja remover esta transação?")) return;

    setError(null);
    try {
      await apiClient.deleteTransaction(id);
      await loadAll();
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <section>
      <header className="page-header">
        <h2>Transações</h2>
        <p>
          Cadastro de receitas e despesas. As regras de menor de idade e de
          finalidade de categoria são aplicadas na API.
        </p>
      </header>

      <form className="card form" onSubmit={handleSubmit}>
        <div
          className="form-grid-transactions"
          style={{
            display: "grid",
            gridTemplateColumns: "1fr 1fr 1fr",
            gap: "1rem",
            alignItems: "end",
          }}
        >
          {role === "Admin" && (
            <label>
              Pessoa
              <PersonSelect
                value={form.personId}
                onChange={(value) =>
                  setForm((f) => ({ ...f, personId: value }))
                }
              />
            </label>
          )}
          <label>
            Descrição
            <input
              type="text"
              value={form.description}
              maxLength={400}
              onChange={(e) =>
                setForm((f) => ({ ...f, description: e.target.value }))
              }
            />
          </label>
          <label>
            Valor
            <input
              type="number"
              min={0}
              step="0.01"
              value={form.amount}
              onChange={(e) =>
                setForm((f) => ({ ...f, amount: Number(e.target.value) }))
              }
            />
          </label>
          <label>
            Categoria
            <CategorySelect
              value={form.categoryId}
              onChange={(value) =>
                setForm((f) => ({ ...f, categoryId: value }))
              }
            />
          </label>
          <label>
            Tipo
            <select
              value={form.type}
              onChange={(e) =>
                setForm((f) => ({
                  ...f,
                  type: e.target.value as TransactionType,
                }))
              }
              style={{ width: "100%", minWidth: 0 }}
            >
              <option value="Expense">Despesa</option>
              <option value="Income">Receita</option>
            </select>
          </label>
          <div
            style={{
              display: "flex",
              gap: "0.5rem",
              justifyContent: "flex-end",
            }}
          >
            <button type="submit" className="primary">
              {editingId == null ? "Registrar transação" : "Salvar alterações"}
            </button>
            {editingId != null && (
              <button
                type="button"
                className="secondary"
                onClick={() => {
                  setEditingId(null);
                  setForm((prev) => ({
                    ...prev,
                    description: "",
                    amount: 0,
                  }));
                }}
              >
                Cancelar edição
              </button>
            )}
          </div>
        </div>
        {error && <p className="error">{error}</p>}
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
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {transactions.map((t) => (
                  <tr key={t.id}>
                    <td>{t.description}</td>
                    <td>{TRANSACTION_LABEL[t.type]}</td>
                    <td>{t.amount.toFixed(2)}</td>
                    <td>{t.personName}</td>
                    <td>
                      <span
                        style={{
                          display: "inline-flex",
                          alignItems: "center",
                          gap: "0.4rem",
                        }}
                      >
                        <span
                          className="tag-color"
                          style={{
                            backgroundColor: t.categoryColorHex ?? "#e5e7eb",
                          }}
                        />
                        <span>{t.categoryDescription}</span>
                      </span>
                    </td>
                    <td>
                      <button type="button" onClick={() => handleEdit(t)}>
                        Editar
                      </button>
                      <button
                        type="button"
                        onClick={() => handleDelete(t.id)}
                        className="danger"
                      >
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
  );
}
