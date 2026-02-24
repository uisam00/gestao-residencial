import { useEffect, useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import {
  apiClient,
  type CategoryPurpose,
  type CategoryTotalsSummaryDto,
  type PersonTotalsSummaryDto,
  type TransactionType,
} from "../../services/ApiClient";
import { PersonSelect } from "../../components/PersonSelect";
import { CategorySelect } from "../../components/CategorySelect";

const PURPOSE_LABEL: Record<CategoryPurpose, string> = {
  Expense: "Despesa",
  Income: "Receita",
  Both: "Ambas",
};

// Tela de relatórios consolidados por pessoa e por categoria.
// Apenas leitura: os totais já vêm calculados pela API a partir das transações.
export function ReportsPage() {
  const [personTotals, setPersonTotals] =
    useState<PersonTotalsSummaryDto | null>(null);
  const [categoryTotals, setCategoryTotals] =
    useState<CategoryTotalsSummaryDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filterPersonId, setFilterPersonId] = useState<number | 0>(0);
  const [filterCategoryId, setFilterCategoryId] = useState<number | 0>(0);
  const [filterType, setFilterType] = useState<TransactionType | "">("");
  const [activeTab, setActiveTab] = useState<"person" | "category">("person");
  const [filtersOpen, setFiltersOpen] = useState(false);
  const [personViewMode, setPersonViewMode] = useState<"chart" | "table">(
    "chart",
  );
  const [categoryViewMode, setCategoryViewMode] = useState<"chart" | "table">(
    "chart",
  );

  const categoryChartData =
    categoryTotals?.items.map((c) => ({
      ...c,
      incomeForChart: c.purpose === "Expense" ? null : c.totalIncome,
      expenseForChart: c.purpose === "Income" ? null : c.totalExpense,
    })) ?? [];

  async function load() {
    setLoading(true);
    setError(null);
    try {
      const filters =
        filterPersonId || filterCategoryId || filterType
          ? {
              personId: filterPersonId || undefined,
              categoryId: filterCategoryId || undefined,
              type: filterType || undefined,
            }
          : undefined;

      const [byPerson, byCategory] = await Promise.all([
        apiClient.getTotalsByPerson(filters),
        apiClient.getTotalsByCategory(filters),
      ]);
      setPersonTotals(byPerson);
      setCategoryTotals(byCategory);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, [filterPersonId, filterCategoryId, filterType]);

  function handleClearFilters() {
    setFilterPersonId(0);
    setFilterCategoryId(0);
    setFilterType("");
  }

  return (
    <section>
      <header className="page-header">
        <h2>Relatórios</h2>
        <p>
          Totais de receitas, despesas e saldo por pessoa e por categoria, além
          dos totais gerais consolidados do sistema.
        </p>
      </header>

      <div className="card form filters-card">
        <button
          type="button"
          className="filters-header w-full text-sm font-medium text-slate-600"
          onClick={() => setFiltersOpen((open) => !open)}
        >
          <span className="filters-title">Filtros</span>
          <span
            className={
              filtersOpen ? "filters-toggle-icon open" : "filters-toggle-icon"
            }
          >
            ▾
          </span>
        </button>

        <div
          className={filtersOpen ? "filters-body open mt-2" : "filters-body"}
        >
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "auto auto auto",
              gap: "1rem 1.5rem",
            }}
          >
            <label>
              Pessoa
              <PersonSelect
                value={filterPersonId}
                onChange={setFilterPersonId}
                allowAllOption
                labelAllText="Todas"
              />
            </label>

            <label>
              Categoria
              <CategorySelect
                value={filterCategoryId}
                onChange={setFilterCategoryId}
                allowAllOption
                labelAllText="Todas"
              />
            </label>

            <label>
              Tipo
              <select
                className="rounded-md border border-slate-300 px-2 py-1 text-sm outline-none focus:border-blue-600 focus:ring-1 focus:ring-blue-500"
                value={filterType}
                onChange={(e) =>
                  setFilterType(e.target.value as TransactionType | "")
                }
              >
                <option value="">Todos</option>
                <option value="Expense">Despesa</option>
                <option value="Income">Receita</option>
              </select>
            </label>

            <div style={{ gridColumn: "1 / -1" }}>
              <button
                type="button"
                className="secondary w-fit rounded-full px-3 py-1 text-xs"
                onClick={handleClearFilters}
              >
                Limpar filtros
              </button>
            </div>
          </div>
        </div>
      </div>

      {error && <p className="error">{error}</p>}
      {loading && <p>Carregando relatórios...</p>}

      <div className="card reports-card">
        <div className="card-header reports-tabs">
          <button
            type="button"
            className={activeTab === "person" ? "tab active" : "tab"}
            onClick={() => setActiveTab("person")}
          >
            Totais por pessoa
          </button>
          <button
            type="button"
            className={activeTab === "category" ? "tab active" : "tab"}
            onClick={() => setActiveTab("category")}
          >
            Totais por categoria
          </button>
        </div>

        {activeTab === "person" && (
          <div className="reports-content">
            {personTotals && personTotals.items.length > 0 ? (
              <>
                <div className="mb-2 flex justify-end gap-2 text-xs">
                  <button
                    type="button"
                    className={
                      personViewMode === "chart"
                        ? "rounded-full bg-blue-600 px-3 py-1 text-xs font-medium text-white"
                        : "rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-700"
                    }
                    onClick={() => setPersonViewMode("chart")}
                  >
                    Ver em gráfico
                  </button>
                  <button
                    type="button"
                    className={
                      personViewMode === "table"
                        ? "rounded-full bg-blue-600 px-3 py-1 text-xs font-medium text-white"
                        : "rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-700"
                    }
                    onClick={() => setPersonViewMode("table")}
                  >
                    Ver em tabela
                  </button>
                </div>
                {personViewMode === "chart" && (
                  <div className="reports-chart-container">
                    <ResponsiveContainer width="100%" height={320}>
                      <BarChart
                        data={personTotals.items}
                        margin={{
                          top: 12,
                          right: 12,
                          left: 12,
                          bottom: 60,
                        }}
                      >
                        <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                        <XAxis
                          dataKey="name"
                          tick={{ fontSize: 12 }}
                          angle={-35}
                          textAnchor="end"
                          height={60}
                        />
                        <YAxis tick={{ fontSize: 12 }} />
                        <Tooltip
                          formatter={(value: any) =>
                            typeof value === "number"
                              ? value.toFixed(2)
                              : value ?? "-"
                          }
                          contentStyle={{
                            borderRadius: "8px",
                            border: "1px solid #e2e8f0",
                          }}
                        />
                        <Legend />
                        <Bar
                          dataKey="totalIncome"
                          name="Receitas"
                          fill="#22c55e"
                          radius={[4, 4, 0, 0]}
                        />
                        <Bar
                          dataKey="totalExpense"
                          name="Despesas"
                          fill="#ef4444"
                          radius={[4, 4, 0, 0]}
                        />
                        <Bar
                          dataKey="balance"
                          name="Saldo"
                          fill="#3b82f6"
                          radius={[4, 4, 0, 0]}
                        />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>
                )}
                {personViewMode === "table" && (
                  <div className="list-scroll reports-list">
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
                            <td
                              className={p.balance >= 0 ? "positive" : "negative"}
                            >
                              {p.balance.toFixed(2)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
                <div className="summary-row reports-summary">
                  <span>
                    Total geral receitas:{" "}
                    {personTotals.grandTotalIncome.toFixed(2)}
                  </span>
                  <span>
                    Total geral despesas:{" "}
                    {personTotals.grandTotalExpense.toFixed(2)}
                  </span>
                  <span
                    className={
                      personTotals.grandBalance >= 0 ? "positive" : "negative"
                    }
                  >
                    Saldo líquido geral: {personTotals.grandBalance.toFixed(2)}
                  </span>
                </div>
              </>
            ) : (
              <p className="reports-empty">
                Nenhuma transação cadastrada para cálculo de totais por pessoa.
              </p>
            )}
          </div>
        )}

        {activeTab === "category" && (
          <div className="reports-content">
            {categoryTotals && categoryTotals.items.length > 0 ? (
              <>
                <div className="mb-2 flex justify-end gap-2 text-xs">
                  <button
                    type="button"
                    className={
                      categoryViewMode === "chart"
                        ? "rounded-full bg-blue-600 px-3 py-1 text-xs font-medium text-white"
                        : "rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-700"
                    }
                    onClick={() => setCategoryViewMode("chart")}
                  >
                    Ver em gráfico
                  </button>
                  <button
                    type="button"
                    className={
                      categoryViewMode === "table"
                        ? "rounded-full bg-blue-600 px-3 py-1 text-xs font-medium text-white"
                        : "rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-700"
                    }
                    onClick={() => setCategoryViewMode("table")}
                  >
                    Ver em tabela
                  </button>
                </div>
                {categoryViewMode === "chart" && (
                  <div className="reports-chart-container">
                    <ResponsiveContainer width="100%" height={320}>
                      <BarChart
                        data={categoryChartData}
                        margin={{
                          top: 12,
                          right: 12,
                          left: 12,
                          bottom: 60,
                        }}
                      >
                        <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                        <XAxis
                          dataKey="description"
                          tick={{ fontSize: 12 }}
                          angle={-35}
                          textAnchor="end"
                          height={60}
                        />
                        <YAxis tick={{ fontSize: 12 }} />
                        <Tooltip
                          formatter={(value: any) =>
                            typeof value === "number"
                              ? value.toFixed(2)
                              : value ?? "-"
                          }
                          contentStyle={{
                            borderRadius: "8px",
                            border: "1px solid #e2e8f0",
                          }}
                        />
                        <Legend />
                        <Bar
                          dataKey="incomeForChart"
                          name="Receitas"
                          fill="#22c55e"
                          radius={[4, 4, 0, 0]}
                        />
                        <Bar
                          dataKey="expenseForChart"
                          name="Despesas"
                          fill="#ef4444"
                          radius={[4, 4, 0, 0]}
                        />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>
                )}
                {categoryViewMode === "table" && (
                  <div className="list-scroll reports-list">
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
                                    backgroundColor: c.colorHex ?? "#e5e7eb",
                                  }}
                                />
                                <span>{c.description}</span>
                              </span>
                            </td>
                            <td>{PURPOSE_LABEL[c.purpose]}</td>
                            <td>
                              {c.purpose === "Expense"
                                ? "-"
                                : c.totalIncome.toFixed(2)}
                            </td>
                            <td>
                              {c.purpose === "Income"
                                ? "-"
                                : c.totalExpense.toFixed(2)}
                            </td>
                            <td
                              className={c.balance >= 0 ? "positive" : "negative"}
                            >
                              {c.balance.toFixed(2)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
                <div className="summary-row reports-summary">
                  <span>
                    Total geral receitas:{" "}
                    {categoryTotals.grandTotalIncome.toFixed(2)}
                  </span>
                  <span>
                    Total geral despesas:{" "}
                    {categoryTotals.grandTotalExpense.toFixed(2)}
                  </span>
                  <span
                    className={
                      categoryTotals.grandBalance >= 0 ? "positive" : "negative"
                    }
                  >
                    Saldo líquido geral:{" "}
                    {categoryTotals.grandBalance.toFixed(2)}
                  </span>
                </div>
              </>
            ) : (
              <p className="reports-empty">
                Nenhuma transação cadastrada para cálculo de totais por
                categoria.
              </p>
            )}
          </div>
        )}

      </div>
    </section>
  );
}
