## Controle de Gastos Residenciais

Sistema para controle de despesas/receitas por pessoa e por categoria, composto por:
- **Backend (.NET)** na pasta `backend`
- **Frontend (React + TypeScript)** na pasta `frontend`

O banco de dados é **SQLite**, gravado no arquivo `Api/Data/app.db`, garantindo que os dados persistam entre reinicializações.

---

## Documentação detalhada

- **Frontend** – ver [`frontend/README.md`](frontend/README.md)
- **Backend** – ver [`backend/README.md`](backend/README.md)

---

## Pré-requisitos gerais

- **Docker** e **Docker Compose** instalados, **ou**
- **.NET SDK 10**
- **Node.js 22+** e **npm**

---

## Executar com Docker (modo desenvolvimento – hot reload)

Esse modo é o mais indicado durante o desenvolvimento, pois recompila automaticamente API e frontend ao salvar arquivos.

Na raiz do repositório (`c:\dev\teste-controle-gastos-residenciais`):

```bash
docker compose -f docker-compose.dev.yml up
```

- API: `http://localhost:5000`
- Frontend (Vite dev server): `http://localhost:5173`

Ao subir os containers de desenvolvimento:
- As **migrations** do backend são aplicadas automaticamente.
- É garantida a existência de um usuário **padrão** para acesso inicial ao sistema:
  - usuário: `admin`
  - senha: `admin`

Para parar:

```bash
docker compose -f docker-compose.dev.yml down
```

---

## Executar com Docker (modo “normal” – sem hot reload)

Esse modo usa imagens otimizadas, sem recarregar código automaticamente.

Na raiz do projeto:

```bash
docker compose up --build
```

- API: `http://localhost:5000`
- Frontend (build estático): `http://localhost:4173`

Para parar:

```bash
docker compose down
```

---

## Executar localmente (sem Docker)

### 1. API (.NET)

No diretório `Api`:

```bash
dotnet run
```

A API ficará disponível em `https://localhost:5001` ou `http://localhost:5000` (conforme configuração padrão do ASP.NET).  
O banco SQLite será criado automaticamente em `Api/Data/app.db` na primeira execução.

Para desenvolvimento com hot reload:

```bash
dotnet watch run
```

### 2. Frontend (React + Vite)

No diretório `frontend`:

```bash
npm install
npm run dev
```

Por padrão o Vite sobe em `http://localhost:5173`.

Para gerar o build de produção:

```bash
npm run build
```

E para testar o build localmente:

```bash
npm run preview
```

---

## Visão geral das funcionalidades de negócio

- **Pessoas**
  - CRUD completo (criar, listar, editar, excluir)
  - Ao excluir uma pessoa, todas as suas transações são removidas (deleção em cascata)
- **Categorias**
  - Criar e listar
  - Campo **Finalidade**: `despesa`, `receita` ou `ambas` (controla que tipo de transação pode usar a categoria)
- **Transações**
  - Criar e listar
  - Validações:
    - Valor **positivo**
    - **Menores de 18 anos** só podem ter **despesas**
    - Categoria deve aceitar o tipo (`despesa` ou `receita`) conforme a finalidade
- **Relatórios**
  - **Totais por pessoa**: total de receitas, despesas e saldo (receitas – despesas) por pessoa e total geral
  - **Totais por categoria**: total de receitas, despesas e saldo por categoria e total geral

