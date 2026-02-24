## Backend - Arquitetura em Camadas

Essa estrutura de pastas segue claramente a ideia de arquitetura em camadas, muito próxima de **Clean Architecture** / **DDD (Domain-Driven Design)**.

A intenção principal é: **separar responsabilidades**, **reduzir acoplamento** e **facilitar testes e manutenção**.

### Visão Geral da Arquitetura

Em termos de projetos (`.csproj`), a estrutura é:

- **Api** → **Application**, **CrossCutting.IoC**
- **CrossCutting.IoC** → **Application**, **Infrastructure**, **Domain**
- **Application** → **Domain**
- **Infrastructure** → **Application**, **Domain**
- **Domain** → *(não depende de ninguém)*

A dependência continua apontando **para dentro (Domain)**. Ou seja, o domínio não conhece as demais camadas, mas todas as outras conhecem o domínio e/ou as abstrações definidas na Application.

### Camada `Domain` (`GastosResidenciais.Domain`)

- Contém as **entidades de negócio**, **enums** e, opcionalmente, **interfaces de domínio**.
- Não conhece detalhes de banco de dados, HTTP, UI ou frameworks.
- Deve ser o núcleo estável do sistema.

### Camada `Application` (`GastosResidenciais.Application`)

- Implementa **casos de uso** / **serviços de aplicação** (por exemplo: cadastro de pessoas, lançamento de transações, cálculo de totais).
- Orquestra chamadas ao domínio e a **abstrações de infraestrutura**, mas evita conter detalhes concretos de persistência (EF Core, drivers, etc.).
- Expõe **DTOs**, **interfaces de serviços** consumidos pela API e a abstração de contexto de dados `Application.Abstractions.IDataContext`.

### Camada `Infrastructure` (`GastosResidenciais.Infrastructure`)

- Implementa o **acesso a dados** (por exemplo, `DbContext` do EF Core, repositórios, migrations).
- Conhece banco de dados, drivers, mapeamentos, etc.
- Depende de `Domain` para mapear entidades e de `Application` para implementar as suas **abstrações**, por exemplo:
  - `IDataContext` → implementado por `DataContext` (EF Core), e registrado no DI como `IDataContext`.

### Camada `CrossCutting.IoC` (`GastosResidenciais.CrossCutting.IoC`)

- Centraliza a **configuração de injeção de dependência (IoC)** e outros aspectos transversais.
- Normalmente expõe métodos de extensão para registrar:
  - `DbContext`,
  - serviços de aplicação,
  - implementações de repositórios,
  - provedores de tempo, log, etc.

### Camada `Api` (`GastosResidenciais.Api`)

- Ponto de entrada HTTP do backend (controllers/endpoints minimal APIs).
- Depende de `Application` e de `CrossCutting.IoC` para **configurar DI** e **expor os casos de uso**.
- Não contém regras de negócio, apenas:
  - mapeamento de rotas,
  - validações superficiais de request/response,
  - integração com middlewares.

---

### Migrations (EF Core)

As migrations ficam em **`backend\src\GastosResidenciais.Infrastructure\Migrations`**. O `DbContext` está no projeto **GastosResidenciais.Infrastructure** e a connection string é lida da **API** (appsettings); por isso os comandos abaixo usam `--project` no projeto de Infrastructure e `--startup-project` na API.

**Pré-requisito:** [EF Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet) (já vem com o SDK; opcionalmente instale como ferramenta global):

```bash
dotnet tool install --global dotnet-ef
```

Execute os comandos a partir da pasta **backend** (raiz da solução).

#### Criar uma nova migration

```bash
dotnet ef migrations add NomeDaMigration --project src/GastosResidenciais.Infrastructure --startup-project src/GastosResidenciais.Api
```

Exemplo: `dotnet ef migrations add AddUserTable`

Os arquivos são gerados em `backend\src\GastosResidenciais.Infrastructure\Migrations`.

#### Aplicar migrations no banco (atualizar o banco)

```bash
dotnet ef database update --project src/GastosResidenciais.Infrastructure --startup-project src/GastosResidenciais.Api
```

Na **execução da aplicação**, o `MigrationHostedService` já aplica as migrations pendentes automaticamente ao subir a API (em ambiente de desenvolvimento/staging, se desejado).

#### Remover a última migration (sem aplicar)

Use apenas se a migration ainda **não** foi aplicada ao banco (ou reverta antes com `database update` para a migration anterior):

```bash
dotnet ef migrations remove --project src/GastosResidenciais.Infrastructure --startup-project src/GastosResidenciais.Api
```

#### Listar migrations

```bash
dotnet ef migrations list --project src/GastosResidenciais.Infrastructure --startup-project src/GastosResidenciais.Api
```

Garanta que a **connection string** `DefaultConnection` em `appsettings.json` (ou `appsettings.Development.json`) da API esteja correta antes de criar ou aplicar migrations.

---

## Execução rápida do backend

Na pasta `backend`:

```bash
dotnet run --project src/GastosResidenciais.Api
```

- API disponível em `http://localhost:5000` (ou `https://localhost:5001`, conforme perfil).
- O banco **SQLite** é criado/atualizado automaticamente em `Api/Data/app.db` (via migrations).
- Ao iniciar a aplicação, o `MigrationHostedService` aplica as migrations pendentes e o `SeedAdminHostedService` garante a existência de um usuário **padrão**:
  - usuário: `admin`
  - senha: `admin`
  
> **Importante:** em ambientes reais, altere a senha do usuário `admin` o quanto antes (ou remova esse seed) por motivos de segurança.

Para desenvolvimento com hot reload:

```bash
dotnet watch run --project src/GastosResidenciais.Api
```

---

## Autenticação e autorização

- A API expõe um endpoint de login que retorna um **JWT** contendo:
  - usuário,
  - `personId` da pessoa associada,
  - papel (`Admin` ou `User`).
- O frontend envia esse token no header `Authorization: Bearer ...` em todas as chamadas.
- Endpoints sensíveis usam **autorização baseada em roles**:
  - `Admin`:
    - pode gerenciar pessoas,
    - pode gerenciar categorias,
    - pode lançar transações para qualquer pessoa.
  - `User`:
    - só enxerga/lança transações para a própria pessoa,
    - não tem acesso aos cadastros de pessoas/categorias.
- As regras de negócio, como:
  - **menores de 18 anos só podem ter despesas**,
  - categoria deve aceitar o tipo de transação (`Expense` / `Income`),
  são validadas nesta camada (serviços de aplicação / domínio), e erros retornam mensagens claras para o frontend.


