## Frontend – Gastos Residenciais

Aplicação web em **React + TypeScript** (Vite) responsável pela interface do sistema de controle de gastos residenciais.

---

## Tecnologias principais

- **React 18** + **TypeScript**
- **Vite** (dev server e build)
- **React Router** (v7) para rotas
- **Tailwind CSS 4** + CSS tradicional em `style.css`
- **Recharts** para gráficos nos relatórios

---

## Scripts básicos

No diretório `frontend`:

- Desenvolvimento:

```bash
npm install
npm run dev
```

- Build de produção:

```bash
npm run build
```

- Pré-visualizar o build:

```bash
npm run preview
```

---

## Estrutura de pastas (simplificada)

- `src/`
  - `App.tsx` – ponto de montagem da aplicação interna.
  - `main.ts` – bootstrap do React/Vite.
  - `routes/router.tsx` – definição das rotas (`/login`, rotas protegidas etc.).
  - `layouts/AppLayout.tsx` – layout com sidebar fixa e área de conteúdo.
  - `context/AuthContext.tsx` – contexto de autenticação/autorização.
  - `components/`
    - `ProtectedRoute.tsx` – wrapper que protege rotas privadas.
    - `PersonSelect.tsx` – select reutilizável de pessoas (busca via API).
    - `CategorySelect.tsx` – select reutilizável de categorias (com bolinha de cor).
  - `modules/`
    - `auth/LoginPage.tsx` – tela de login.
    - `people/PeoplePage.tsx` – CRUD de pessoas.
    - `categories/CategoriesPage.tsx` – CRUD de categorias.
    - `transactions/TransactionsPage.tsx` – cadastro + listagem de transações.
    - `reports/ReportsPage.tsx` – relatórios (tabelas + gráficos).
  - `services/ApiClient.ts` – cliente HTTP centralizado para a API .NET.
  - `style.css` – tema global, layout da sidebar, tabelas, formulários e ajustes finos.

---

## Módulos de tela

### Login (`modules/auth/LoginPage.tsx`)

- Formulário simples de usuário/senha.
- Usa `useAuth().login(username, password)` para autenticar.
- **Redirecionamento pós-login:**
  - `Admin` → `/people`
  - `User` → `/reports`

### Pessoas (`modules/people/PeoplePage.tsx`)

- CRUD completo de pessoas.
- Para **admin**:
  - Pode criar/editar usuário vinculado (username, senha, perfil Admin/User).
  - Ao excluir uma pessoa, todas as transações associadas são removidas (regra da API).
- A listagem utiliza tabela com header fixo e scroll apenas nas linhas.

### Categorias (`modules/categories/CategoriesPage.tsx`)

- CRUD de categorias com:
  - `Descrição`
  - `Finalidade`: `Despesa`, `Receita` ou `Ambas`
  - `Cor` (`colorHex`) usada como tag/bullet colorido em várias telas.
- A listagem mostra a cor como bolinha (`tag-color`) + descrição e finalidade.

### Transações (`modules/transactions/TransactionsPage.tsx`)

- Cadastro de **receitas** e **despesas** por pessoa e categoria.
- Validações alinhadas com a API:
  - Descrição obrigatória.
  - Valor deve ser positivo.
  - Pessoa obrigatória (para admin) e categoria obrigatória.
- Comportamento por papel:
  - **Admin**:
    - Escolhe livremente a pessoa no formulário.
    - Pode cadastrar transações para qualquer pessoa.
  - **User**:
    - Pessoa é fixada na própria pessoa logada (preenchida a partir do `AuthContext`).
    - Não escolhe outras pessoas.
- Layout:
  - Primeira linha: Pessoa | Descrição | Valor.
  - Segunda linha: Categoria | Tipo | botão de ação.
  - Histórico com tabela, header fixo e scroll somente na área de registros.

### Relatórios (`modules/reports/ReportsPage.tsx`)

- Filtros:
  - Pessoa, Categoria, Tipo (Despesa/Receita), com opção de limpar filtros.
  - Filtros colapsáveis (abrem/fecham com animação).
- Abas:
  - **Totais por pessoa** – tabela + opção de ver em gráfico.
  - **Totais por categoria** – tabela + opção de ver em gráfico.
- Gráficos:
  - Implementados com **Recharts**.
  - Por pessoa: barras de Receitas, Despesas e Saldo.
  - Por categoria: barras de Receitas e Despesas (saldo só na tabela).
  - Os gráficos respeitam os mesmos filtros da tabela.

---

## Autenticação e autorização

### Fluxo de autenticação

- O contexto `AuthContext` guarda:
  - `token` (JWT recebido da API),
  - `username` (nome da pessoa),
  - `role` (`Admin` ou `User`),
  - `personId` (id da pessoa logada).
- Esses dados são persistidos em `localStorage` usando as chaves:
  - `gastos_token`, `gastos_username`, `gastos_role`, `gastos_person_id`.
- No login:
  1. `apiClient.login(user, password)` retorna o **token**.
  2. O token é salvo e aplicado no cliente HTTP (`Authorization: Bearer ...`).
  3. Em seguida é chamado `apiClient.getMe()` para obter `personName`, `role` e `personId`.
  4. O contexto atualiza o estado global e o `localStorage`.

### Validação automática de sessão

- Ao carregar a aplicação:
  - Se existir `gastos_token`, ele é aplicado no `ApiClient`.
  - O frontend chama `getMe` para validar o token:
    - Se OK: atualiza `username`, `role`, `personId`.
    - Se falhar: limpa token e dados de sessão.

### Proteção de rotas

- `ProtectedRoute`:
  - Enquanto `loading` é `true`, mostra um estado de “Carregando…”.
  - Se não há `token`, redireciona para `/login`.
  - Caso contrário, renderiza o conteúdo protegido.
- `router.tsx`:
  - `/login` é a única rota pública.
  - Qualquer outra rota é envolvida por `ProtectedRoute` + `AppLayout`.

### Regras de autorização na UI

- **Sidebar (`AppLayout`)**:
  - Para **Admin**: mostra links para `Pessoas`, `Categorias`, `Transações`, `Relatórios`.
  - Para **User**: mostra apenas `Transações` e `Relatórios`.
- **Transações**:
  - Para **Admin**: campo `Pessoa` visível e obrigatório.
  - Para **User**: pessoa é fixada no usuário logado (`personId` vindo do backend); o campo não é exibido.
- Regras de negócio mais sensíveis (ex.: menor de 18 anos só despesa) são validadas principalmente no **backend**, com o frontend apenas refletindo mensagens de erro.

---

## Estilos e layout

- A base visual é controlada por `src/style.css`:
  - Layout com sidebar fixa (`.app-shell`, `.sidebar`, `.content`).
  - Estilos comuns de `.card`, `.table`, formulários e botões.
  - Scroll somente nas áreas de listagem (divs com classe `.list-scroll`).
- Tailwind é utilizado em pontos específicos (principalmente para espaçamentos e alinhamentos finos) junto com as classes CSS já existentes.

