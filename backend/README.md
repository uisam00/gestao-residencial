## Backend - Arquitetura em Camadas

Essa estrutura de pastas segue claramente a ideia de arquitetura em camadas, muito próxima de **Clean Architecture** / **DDD (Domain-Driven Design)**.

A intenção principal é: **separar responsabilidades**, **reduzir acoplamento** e **facilitar testes e manutenção**.

### Visão Geral da Arquitetura

API  →  Application  →  Domain  
     ↓  
    Infrastructure (Data)  
     ↓  
 CrossCutting (IoC)

A dependência deve sempre apontar **para dentro (Domain)**. Ou seja, o domínio não conhece as demais camadas, mas todas as outras conhecem o domínio.

### Camada `Domain` (`GastosResidenciais.Domain`)

- Contém as **entidades de negócio**, **enums** e, opcionalmente, **interfaces de domínio**.
- Não conhece detalhes de banco de dados, HTTP, UI ou frameworks.
- Deve ser o núcleo estável do sistema.

### Camada `Application` (`GastosResidenciais.Application`)

- Implementa **casos de uso** / **serviços de aplicação** (por exemplo: cadastro de pessoas, lançamento de transações, cálculo de totais).
- Orquestra chamadas ao domínio e à infraestrutura, mas evita conter regras complexas de persistência.
- Expõe **DTOs** e **interfaces de serviços** consumidos pela API.

### Camada `Infrastructure` (`GastosResidenciais.Infrastructure`)

- Implementa o **acesso a dados** (por exemplo, `DbContext` do EF Core, repositórios, migrations).
- Conhece banco de dados, drivers, mapeamentos, etc.
- Depende de `Domain` para mapear entidades.

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

