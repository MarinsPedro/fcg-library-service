# FiapCloudGames.Library

Microservico responsavel pela biblioteca de jogos dos usuarios do Fiap Cloud Games.

## Responsabilidade

O Library responde quais jogos um usuario possui. Ele nao calcula preco, nao processa pagamento e nao conclui pedidos.

Fluxo principal:

```text
Orders --HTTP--> Library: usuario ja possui o jogo?
Orders Saga --GameGrantRequestedEventV1--> Library Worker
Library Worker --> cria LibraryEntry
Library Worker --GameGrantedEventV1--> Orders Saga
Orders Saga --OrderCompletedEventV1--> Notifications Worker
```

## Rotas HTTP

### Verificar ownership

```http
GET /internal/v1/library/users/{userId}/games/{gameId}
```

Resposta:

```json
true
```

Esta e a consulta usada pelo Orders antes de iniciar uma nova compra.

Scope: `library.ownership.read`.

### Listar biblioteca do usuario

```http
GET /v1/library/users/{userId}/games?limit=20&cursor=...
```

A listagem usa cursor composto por `GrantedAtUtc + Id`, evitando paginacao por offset.

Scope: `library.games.read`.


## Mensageria

O Library utiliza MassTransit com RabbitMQ e Entity Framework Consumer Outbox.

### Entrada - Solicitação de concessão

- Contrato: `GameGrantRequestedEventV1`
- Exchange: `library.game-grant-requested.v1` (fanout MassTransit)
- Fila: `fcg-library-game-grant-requested`
- Campos: `EventId`, `OrderId`, `UserId`, `GameId`, `OccurredAtUtc`

O Worker recebe o evento, concede o jogo pelo `GrantGameService` e registra a operação de forma idempotente.

### Saída - Jogo concedido

- Contrato: `GameGrantedEventV1`
- Exchange: `library.game-granted.v1` (fanout MassTransit)
- Campos: `EventId`, `OrderId`, `UserId`, `GameId`, `GrantedAtUtc`

Orders recebe a confirmação e prossegue com a saga. O MassTransit declara exchanges, filas e bindings automaticamente, sem Job de topologia externa ou routing keys manuais.

### Confiabilidade

- Consumer Outbox com PostgreSQL
- Idempotência de negócio por OrderId
- Unicidade de ownership por UserId e GameId
- CorrelationId propagado quando disponível
- Retry para falhas transitórias

Os contratos V1 ficam em `FiapCloudGames.Library.Contracts.Events.V1`.

## Persistencia

Tabela de dominio:

```text
library.library_entries
- id
- user_id
- game_id
- order_id
- granted_at_utc
```

Regras de unicidade:

```text
UNIQUE (user_id, game_id)
UNIQUE (order_id)
```

Tabelas de mensageria do MassTransit:

```text
library.inbox_state
library.outbox_message
library.outbox_state
```

## Variaveis de ambiente

No ambiente do projeto os valores vem do orquestrador (`config/*-library-*.env` e Secrets); o `appsettings.json` do servico fica vazio. `__` equivale a `:` na configuracao do .NET.

### API (`library-api`)

| Variavel | Obrigatoria | Onde fica no cluster | Exemplo | Para que serve |
|---|---|---|---|---|
| `ConnectionStrings__Database` | sim | Secret (`fcg-dev-secrets`, `library-connection-string`) | `Host=postgres;Database=fcg_library;...` | Conexao com o PostgreSQL |
| `Authentication__Authority` | sim | ConfigMap | `http://identity-api:8080` | Identity que valida os tokens |
| `Authentication__Audience` | sim | ConfigMap | `library-api` | Audience aceita |
| `Authentication__Issuer` | sim | ConfigMap | `fiap-cloud-games-identity` | Emissor esperado |
| `Authentication__AllowedTokenUses__0` | sim | ConfigMap | `service` | So tokens de servico |
| `Authentication__RequireHttpsMetadata` | nao | ConfigMap | `false` | `false` em ambiente local |
| `ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS` | nao | ConfigMap | `Development`, `http://+:8080` | Ambiente e porta |

A API nao usa RabbitMQ.

### Worker (`library-worker`)

| Variavel | Obrigatoria | Onde fica | Exemplo | Para que serve |
|---|---|---|---|---|
| `ConnectionStrings__Database` | sim | Secret | - | Banco, entradas da biblioteca e Outbox |
| `RabbitMq__Host` | sim | ConfigMap | `rabbitmq` | Host do RabbitMQ |
| `RabbitMq__Port`, `RabbitMq__VirtualHost` | nao | ConfigMap | `5672`, `/` | Porta e virtual host |
| `RabbitMq__Username`, `RabbitMq__Password` | sim | Secret (`fcg-dev-secrets`) | - | Credenciais do broker |
| `RabbitMq__Topology__Exchanges__GameGrantRequested` | sim | ConfigMap | `library.game-grant-requested.v1` | Exchange do pedido de concessao |
| `RabbitMq__Topology__Exchanges__GameGranted` | sim | ConfigMap | `library.game-granted.v1` | Exchange do jogo concedido |
| `RabbitMq__Topology__Queues__GameGrantRequested` | sim | ConfigMap | `fcg-library-game-grant-requested` | Fila de entrada |
| `DOTNET_ENVIRONMENT` | nao | ConfigMap | `Development` | Ambiente do Worker |

### Migrations (`library-migrations`)

| Variavel | Obrigatoria | Onde fica | Para que serve |
|---|---|---|---|
| `ConnectionStrings__Database` | sim | Secret | Conexao usada para aplicar as migrations |
| `DOTNET_ENVIRONMENT` | nao | ConfigMap | Ambiente do executavel |

Os manifests em `k8s/` se chamam `library-api.yaml`, `library-worker.yaml` e `library-migrations-job.yaml`. ConfigMaps e Secrets sao criados pelo instalador do orquestrador (`fgc-orchestrator-service`, clonado como a pasta `fcg-orchestration`).

## Estrutura

```text
src/
  FiapCloudGames.Library.Api
  FiapCloudGames.Library.Application
  FiapCloudGames.Library.Contracts
  FiapCloudGames.Library.Domain
  FiapCloudGames.Library.Infrastructure
  FiapCloudGames.Library.Migrations
  FiapCloudGames.Library.Presentation
  FiapCloudGames.Library.Worker

tests/
  FiapCloudGames.Library.UnitTests
  FiapCloudGames.Library.ArchitectureTests
  FiapCloudGames.Library.Database.IntegrationTests
  FiapCloudGames.Library.IntegrationTests
```

O desenho segue principalmente o padrao do Catalog, com testes de contrato HTTP inspirados no Promotions e mensageria MassTransit no Worker.

## Migration inicial

Depois de configurar o `NuGet.Config` e restaurar os pacotes, gere a primeira migration com:

```powershell
dotnet ef migrations add DbInit `
  --project .\src\FiapCloudGames.Library.Migrations `
  --startup-project .\src\FiapCloudGames.Library.Migrations
```

A migration inicial deve criar `library_entries` e as tres tabelas do MassTransit.
