# Gerenciador de Processos - Backend

Nesse diretório temos a API do **ProcessoTop**, feita para gerenciar processos judiciais/administrativos, partes envolvidas e andamentos.

## Tecnologias

- **C# / .NET 10** (ASP.NET Core Web API)
- **Entity Framework Core 10** (ORM)
- **PostgreSQL 16** (Banco de dados relacional)
- **FluentValidation** (Validação de DTOs e regras de entrada)
- **xUnit, Moq e FluentAssertions** (Testes automatizados)

## Arquitetura

O projeto foi estruturado seguindo os princípios de **Clean Architecture**. A ideia principal é manter o código testável, organizado e isolar as regras de negócio das dependências externas, como banco de dados ou outras bibliotecas.

As camadas separadas são:

- **Domain**: Onde ficam as regras de negócios. Nessa parte ficam as Entidades (Processo, Parte, Andamento), Enums e Exceções customizadas.
- **Application**: Onde fica a lógica de orquestração, com os Services, interfaces, validators e DTOs. Nessa parte, os DTOs são usados para garantir que os contratos são bem definidos entre as camadas da API, blindando as entidades.
- **Infrastructure**: Implementação do acesso ao banco de dados com EF Core, repositórios e qualquer comunicação externa.
- **API**: A camada HTTP, que expõe os Controllers, configura injeção de dependência e lida com os middlewares, como o tratamento de erros global.

## Como buildar

Para compilar o projeto inteiro, certifique-se de ter o .NET 10 instalado, entre na pasta `backend` e rode:

```bash
dotnet build
```

## Como rodar

A maneira recomendada e mais fácil de rodar o projeto inteiro (banco de dados, API e frontend) é utilizando o Docker Compose. 

As instruções de como rodar com Docker estão no **[README principal](../README.md)**.

## Como rodar testes

A suíte de testes unitários cobre as lógicas e serviços do sistema. Para executá-los, basta rodar na raiz do backend:

```bash
dotnet test
```

## Variáveis de Ambiente

As variáveis principais que a API consome (injetadas via `.env` pelo Docker ou localmente) são:

| Variável | Descrição |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Controla o ambiente (`Development` ou `Production`). Em desenvolvimento, o Swagger e os logs detalhados ficam ativos. |
| `ConnectionStrings__DefaultConnection` | A string de conexão completa com o PostgreSQL (usada pelo EF Core). |

*(Obs: As variáveis `POSTGRES_USER` e `POSTGRES_PASSWORD` são usadas pelo Postgres na hora de criar o banco, e não diretamente pela API).*

## Endpoints Principais

Abaixo estão as rotas expostas pela API e o que elas fazem:

| Endpoint | Descrição |
| --- | --- |
| `/api/processos` | Operações gerais de processos. |
| `/api/processos/{id}` | Operações sobre um processo específico. |
| `/api/processos/{id}/partes` | Operações sobre as partes (polos ativo e passivo) de um processo. |
| `/api/processos/{id}/andamentos` | Operações sobre os andamentos de um processo. |
| `/api/entidades-legais` | Operações gerais de entidades legais (pessoas físicas ou jurídicas). |
| `/api/entidades-legais/{id}` | Operações sobre uma entidade legal específica. |

*(Obs: Como estamos falando de uma aplicação destinada a coisas jurídicas, os DELETE não apagam as entidades no banco de dados. Ao invés disso, eles fazem um Soft Delete, ou seja, as entidades não são mais retornadas pela API, mas permanecem no banco para auditoria).*

## Regras de negócio

1. **Processo:**
   - Se for um processo do tipo Judicial, o número deve seguir a formatação do CNJ (`NNNNNNN-DD.AAAA.J.TR.OOOO`).
   - As transições de estado do Processo são validadas, e só é permitido o fluxo normal. Transições inválidas: Finalizado -> Ativo, e Arquivado -> Finalizado. Transições válidas: Ativo -> Finalizado, Ativo -> Arquivado, Finalizado -> Arquivado, Arquivado -> Ativo.
   - Todo processo precisa ter no mínimo um **Polo Ativo** e um **Polo Passivo**, mas podem ter mais de um.
   - O número do processo precisa ser único.

2. **Andamentos (Movimentações):**
   - A data do andamento **nunca** pode ser no futuro.
   - A data do andamento não pode ser anterior à data de criação do próprio processo.

3. **Arquivamento e Ciclo de Vida:**
   - Processos com status Arquivado ficam congelados. A API bloqueia a inserção de novas partes ou de novos andamentos até que o processo volte a ser Ativo.
