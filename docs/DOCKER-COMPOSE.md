# Docker Compose - Ambientes de Desenvolvimento e Produção

Este projeto possui dois arquivos `docker-compose` separados para diferentes ambientes.

## Desenvolvimento (`docker-compose.dev.yml`)

**Características:**
- ✅ Hot reload habilitado (alterações no código são refletidas automaticamente)
- ✅ Volumes montados para código fonte
- ✅ Kafka UI disponível na porta 8081
- ✅ Ambiente `Development`
- ✅ Logs detalhados

**Como usar:**

```bash
# Subir ambiente de desenvolvimento
docker compose -f docker-compose.dev.yml up --build

# Subir em background
docker compose -f docker-compose.dev.yml up -d --build

# Ver logs
docker compose -f docker-compose.dev.yml logs -f api

# Parar ambiente
docker compose -f docker-compose.dev.yml down

# Parar e remover volumes
docker compose -f docker-compose.dev.yml down -v
```

**Serviços disponíveis:**
- API: http://localhost:8080
- Kafka UI: http://localhost:8081
- PostgreSQL: localhost:5432
- Kafka: localhost:9092

## Produção (`docker-compose.prod.yml`)

**Características:**
- ✅ Build otimizado (multi-stage)
- ✅ Sem hot reload (imagem otimizada)
- ✅ Health checks configurados
- ✅ Restart policies (`unless-stopped`)
- ✅ Variáveis de ambiente configuráveis
- ✅ Ambiente `Production`

**Como usar:**

```bash
# Criar arquivo .env com variáveis de ambiente (opcional)
# POSTGRES_DB=consultacreditos
# POSTGRES_USER=postgres
# POSTGRES_PASSWORD=senha_segura
# POSTGRES_PORT=5432
# KAFKA_PORT_PLAINTEXT=9092
# KAFKA_PORT_CONTROLLER=9093
# KAFKA_EXTERNAL_HOST=seu-host
# API_PORT=8080

# Subir ambiente de produção
docker compose -f docker-compose.prod.yml up --build -d

# Ver logs
docker compose -f docker-compose.prod.yml logs -f api

# Verificar status dos serviços
docker compose -f docker-compose.prod.yml ps

# Parar ambiente
docker compose -f docker-compose.prod.yml down

# Parar e remover volumes (CUIDADO: apaga dados!)
docker compose -f docker-compose.prod.yml down -v
```

**Variáveis de Ambiente (Produção):**

Todas as variáveis têm valores padrão, mas podem ser sobrescritas via arquivo `.env`:

- `POSTGRES_DB`: Nome do banco de dados (padrão: `consultacreditos`)
- `POSTGRES_USER`: Usuário do PostgreSQL (padrão: `postgres`)
- `POSTGRES_PASSWORD`: Senha do PostgreSQL (padrão: `postgres`)
- `POSTGRES_PORT`: Porta do PostgreSQL (padrão: `5432`)
- `KAFKA_PORT_PLAINTEXT`: Porta do Kafka (padrão: `9092`)
- `KAFKA_PORT_CONTROLLER`: Porta do controller do Kafka (padrão: `9093`)
- `KAFKA_EXTERNAL_HOST`: Host externo do Kafka (padrão: `localhost`)
- `API_PORT`: Porta da API (padrão: `8080`)

## Diferenças Principais

| Característica | Dev | Prod |
|---------------|-----|------|
| Dockerfile | `Dockerfile.dev` | `Dockerfile` |
| Hot Reload | ✅ Sim | ❌ Não |
| Volumes | ✅ Montados | ❌ Não |
| Kafka UI | ✅ Incluído | ❌ Não |
| Ambiente | Development | Production |
| Restart Policy | Não | unless-stopped |
| Health Checks | Básicos | Completos |
| Build | SDK completo | Multi-stage otimizado |

## Troubleshooting

### Porta já em uso
Se alguma porta estiver em uso, altere no arquivo `docker-compose` correspondente ou via variáveis de ambiente.

### Volumes diferentes
Os ambientes usam volumes separados:
- Dev: `postgres_data_dev`
- Prod: `postgres_data_prod`

Isso evita conflitos entre ambientes.

### Health checks falhando
No ambiente de produção, os health checks podem levar alguns segundos para passar. Aguarde até que todos os serviços estejam `healthy`.

