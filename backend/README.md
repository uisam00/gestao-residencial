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

