## Regras de Negócio — Consulta de Créditos (ISSQN)

Este documento descreve **as regras de negócio e comportamentos funcionais** implementados no microserviço.

### Conceitos do domínio

- **Crédito constituído (ISSQN)**: valor tributário oficialmente lançado e juridicamente exigível.
- **Vínculo com NFS-e**: todo crédito está associado a uma **NFS-e** (Nota Fiscal de Serviços Eletrônica), identificada por `numeroNfse`.
- **Chaves de identificação**:
  - **`numeroCredito`**: identificador único do crédito (unicidade/ idempotência).
  - **`numeroNfse`**: identificador da NFS-e (pode ter múltiplos créditos).

### Entidade principal: `credito`

- **Unicidade**: `numero_credito` é **único** (índice único).
- **Persistência**: cada crédito é inserido **individualmente** (sem bulk).

### Mensageria (Kafka)

- **Tópico de integração**: `integrar-credito-constituido-entry`
  - Recebe 1 mensagem por crédito a ser integrado.
- **Tópico de auditoria**: `consulta-creditos-audit`
  - Recebe eventos de auditoria de **consultas** e também da **integração (POST)**.

### API — Endpoints e regras

#### 1) POST `/api/Creditos/integrar-credito-constituido`

**Objetivo**: receber uma lista de créditos e publicar **uma mensagem por crédito** no Kafka.

- **Entrada**: lista de créditos (JSON array).
- **Validação**:
  - Se a lista vier vazia/nula: retorna **400 BadRequest**.
- **Regra principal**:
  - Para cada item recebido, publica **1 mensagem** no tópico `integrar-credito-constituido-entry`.
- **Resposta**:
  - Retorna **202 Accepted** com `{ "success": true }`.
- **Auditoria do POST (implementada)**:
  - Após publicar todos os créditos, publica um evento no tópico `consulta-creditos-audit` com:
    - `TipoConsulta = "IntegracaoCredito"`
    - `ChaveConsulta` com identificador gerado (inclui quantidade + timestamp)
    - `CorrelationId` contendo a lista de `numeroCredito` enviados (separados por vírgula)
  - **Importante**: falha ao auditar **não** invalida o POST (apenas loga warning).

#### 2) GET `/api/Creditos/{numeroNfse}`

**Objetivo**: retornar todos os créditos associados a uma NFS-e.

- **Regra principal**:
  - Consulta no banco por `numero_nfse = numeroNfse`.
  - Retorna a lista (pode ser vazia).
- **Auditoria do GET**:
  - Publica evento no tópico `consulta-creditos-audit`:
    - `TipoConsulta = "PorNfse"`
    - `ChaveConsulta = numeroNfse`

#### 3) GET `/api/Creditos/credito/{numeroCredito}`

**Objetivo**: retornar o detalhe de um crédito pelo seu número.

- **Regra principal**:
  - Consulta no banco por `numero_credito = numeroCredito`.
  - Se não existir: retorna **404 NotFound**.
  - Se existir: retorna **200 OK** com o crédito.
- **Auditoria do GET**:
  - Publica evento no tópico `consulta-creditos-audit`:
    - `TipoConsulta = "PorCredito"`
    - `ChaveConsulta = numeroCredito`

### Background Services — Regras de processamento assíncrono

#### 1) Consumer de integração (`CreditoIntegrationConsumerService`)

- **Objetivo**: consumir mensagens do tópico `integrar-credito-constituido-entry` e persistir no banco.
- **Polling**:
  - Loop contínuo, com **delay de ~500ms** quando não há mensagem.
- **Processamento**:
  - Processa **uma mensagem por vez**.
  - Realiza **commit manual** do offset após conclusão (ou descarte controlado).
- **Idempotência (regra crítica)**:
  - Antes de inserir, verifica se já existe `numero_credito`.
  - Se já existir:
    - Não insere novamente.
    - Faz commit e segue (processamento idempotente).
  - Backup de idempotência por restrição:
    - Existe **índice único** em `numero_credito`.
    - Se ocorrer race condition e der violação de unique, o serviço trata como idempotente e faz commit.

#### 2) Consumer de auditoria (`AuditoriaConsumerService`)

- **Objetivo**: consumir mensagens do tópico `consulta-creditos-audit` e persistir no banco.
- **Polling**:
  - Loop contínuo, com **delay de ~500ms** quando não há mensagem.
- **Persistência**:
  - Persiste em `auditoria_consulta`.
  - Commit manual após salvar.

### Auditoria — Endpoints e regras

> Observação: os eventos são publicados no Kafka e **persistidos** pelo consumer de auditoria; os endpoints abaixo consultam o banco.

- **GET `/api/Auditoria`**: lista todas as auditorias (mais recentes primeiro).
- **GET `/api/Auditoria/{id}`**: busca auditoria por ID (404 se não existir).
- **GET `/api/Auditoria/tipo/{tipoConsulta}`**: filtra por tipo (ex.: `PorNfse`, `PorCredito`, `IntegracaoCredito`).
- **GET `/api/Auditoria/chave/{chaveConsulta}`**: filtra por chave (ex.: número NFS-e, número crédito, ou chave gerada do POST).

### Saúde do serviço

- **GET `/self`**: liveness (processo “de pé”).
- **GET `/ready`**: readiness (dependências críticas OK: banco + Kafka).


