## Contexto no mundo real — Consulta e Integração de Créditos (ISSQN)

Este documento explica **por que** este microserviço existe e **como** ele se encaixa em um cenário real de negócios e arquitetura.

### O que é ISSQN e por que “crédito constituído” importa

- **ISSQN** é o imposto municipal sobre serviços.
- Um **crédito constituído** representa um valor de ISSQN **lançado** (formalizado) e, portanto, **exigível** do contribuinte.
- Esses créditos normalmente nascem de processos como:
  - emissão/validação de **NFS-e**;
  - apurações e fiscalizações;
  - retificações;
  - integrações com ERPs/portais municipais.

Em ambientes corporativos/municipais, esse tipo de dado é usado para:
- conciliação de arrecadação e fiscalização;
- controles de dívida ativa/ cobranças;
- relatórios e auditorias internas e externas;
- rastreabilidade de consultas e integrações.

### Problema real que o serviço resolve

Em produção, a origem dos créditos pode ser **assíncrona** e de **alto volume** (ex.: integrações, lotes, eventos de terceiros).
Ao mesmo tempo, os consumidores dos dados (portais, backoffice, BI, integrações) precisam de:
- **consultas rápidas** por NFS-e e por número do crédito;
- **idempotência** (não duplicar créditos quando o mesmo evento é reenviado);
- **auditoria** (quem consultou o quê, quando, por qual chave).

### Por que usar mensageria + processamento assíncrono

No mundo real, publicar no Kafka e processar em background traz:
- **desacoplamento**: a API não precisa “segurar” o tempo de escrita no banco para cada crédito.
- **resiliência**: se o banco estiver temporariamente indisponível, o evento pode ser reprocessado.
- **elasticidade**: é possível escalar consumidores para lidar com picos.
- **observabilidade**: offsets/partitions facilitam rastreio de processamento.

### Eventual consistency (consistência eventual)

O fluxo “POST publica no Kafka → consumer grava no banco” implica que:
- a API confirma recebimento (**202 Accepted**) antes do dado estar necessariamente visível no banco;
- o dado aparece após o consumer processar.

Isso é comum em arquitetura orientada a eventos, e faz sentido quando:
- o processamento pode demorar;
- há necessidade de absorver picos sem degradar a API.

### Por que idempotência é essencial

Em produção, é comum o mesmo evento chegar mais de uma vez por:
- retry do produtor;
- reprocessamento por falha temporária;
- duplicidade upstream.

Sem idempotência, o serviço poderia:
- duplicar créditos (erro grave em domínio fiscal);
- causar inconsistências em relatórios e cobranças.

Aqui a idempotência é garantida por:
- **verificação de existência** antes de inserir (otimização);
- **índice único** no banco (garantia forte).

### Auditoria (log de eventos) como requisito real

Em sistemas fiscais, auditoria costuma ser requisito de:
- conformidade e governança;
- investigações internas (ex.: “quem consultou tal crédito?”);
- correlação com incidentes (“houve pico de consultas?”).

Neste serviço, auditoria é implementada via:
- publicação de evento no tópico `consulta-creditos-audit` quando ocorrem:
  - consultas por NFS-e;
  - consultas por crédito;
  - integração via POST (auditoria do ato de integrar).
- persistência desses eventos em tabela própria, permitindo consulta via endpoints.

### Health checks e prontidão em produção

Em Kubernetes/containers, é comum separar:
- **liveness** (`/self`): processo está vivo e respondendo.
- **readiness** (`/ready`): dependências críticas (Kafka e banco) estão operacionais.

Isso evita que um pod/container receba tráfego antes de estar realmente pronto.

### Como esse serviço se conecta ao ecossistema

Exemplos realistas de integrações:
- **Produtores** (publicam créditos a integrar):
  - sistemas de emissão/validação de NFS-e;
  - backoffice fiscal;
  - integrações com ERPs.
- **Consumidores** (consultam e auditam):
  - portal do contribuinte;
  - atendimento/operadores;
  - BI/analytics (por meio de eventos ou leitura do banco).

### Considerações de produção (boas práticas)

Em um cenário real, evoluções comuns seriam:
- DLQ (dead-letter) para mensagens “venenosas” após N tentativas;
- métricas (latência por endpoint, lag do consumer, taxa de inserts);
- autenticação/autorização nos endpoints (principalmente auditoria);
- mascaramento/controle de dados sensíveis conforme LGPD e políticas internas;
- versionamento de contratos de mensagens.


