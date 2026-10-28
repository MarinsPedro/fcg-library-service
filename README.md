# FiapCloudGames.Library

Microservico responsavel pela biblioteca de jogos dos usuarios do Fiap Cloud Games.

## Responsabilidade

O Library responde quais jogos um usuario possui. Ele nao calcula preco, nao processa pagamento e nao conclui pedidos.

Fluxo principal:

```text
Orders --HTTP--> Library: usuario ja possui o jogo?

Payments --PaymentPaidEvent--> Orders
Orders --OrderCompletedEvent--> Library Worker
Library Worker --> cria LibraryEntry
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

O Library utiliza MassTransit com RabbitMQ e
Entity Framework Consumer Outbox.

### Entrada — Solicitação de concessão

Contrato: GameGrantRequestedEventV1

Exchange: fcg.events
Tipo: topic
Routing key: library.game.grant.requested
Fila: fcg.library.game-grant-requested

Payload:
- OrderId
- UserId
- GameId
- OccurredAtUtc

O Worker recebe o evento e solicita a concessão
do jogo ao GrantGameService.

### Saída — Jogo concedido

Contrato: GameGrantedEventV1

Exchange: fcg.library.events
Tipo: topic
Routing key: library.game.granted

Payload:
- LibraryEntryId
- OrderId
- UserId
- GameId
- GrantedAtUtc

Após a concessão, o Library publica a confirmação
para que Orders possa concluir o pedido.

### Versionamento

Os contratos estão em:

FiapCloudGames.Library.Contracts.Events.V1

### Confiabilidade

- Consumer Outbox com PostgreSQL
- Controle de mensagens recebidas pelo MassTransit
- Idempotência de negócio por OrderId
- Unicidade de ownership por UserId e GameId
- CorrelationId propagado quando disponível
- Retry para falhas transitórias

### Integração pendente

Alinhar contratos, routing keys e bindings com Orders.
Validar o fluxo com RabbitMQ e PostgreSQL.


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
