# GitHub Actions - Docker Pipeline

Este projeto possui um pipeline automatizado para build e publicação de imagens Docker no GitHub Container Registry (GHCR).

## Workflow: `docker-publish.yml`

### Triggers

O pipeline é executado automaticamente quando:

1. **Push na branch `main`**: Build e push da imagem com tag `main` e `latest`
2. **Criação de tag**: Quando uma tag no formato `v*` é criada (ex: `v1.0.0`)
3. **Pull Request**: Apenas build (sem push) para validação
4. **Manual**: Via `workflow_dispatch` com tag customizada opcional

### Como Usar

#### Publicação Automática

1. **Push na main**:
   ```bash
   git push origin main
   ```
   - A imagem será publicada automaticamente como `ghcr.io/seu-usuario/consulta-creditos-dotnet-6-backend:main`
   - E também como `:latest` se for a branch padrão

2. **Criar uma release**:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
   - A imagem será publicada com tags: `v1.0.0`, `1.0`, `1`, `latest`

#### Publicação Manual

1. Vá para **Actions** no GitHub
2. Selecione **"Build and Push Docker Image to GHCR"**
3. Clique em **"Run workflow"**
4. Opcionalmente, informe uma tag customizada
5. Execute

### Imagem Publicada

A imagem será publicada em:
```
ghcr.io/[seu-usuario]/consulta-creditos-dotnet-6-backend:[tag]
```

### Como Usar a Imagem Publicada

#### Pull da Imagem

```bash
# Login no GHCR (primeira vez)
echo $GITHUB_TOKEN | docker login ghcr.io -u [seu-usuario] --password-stdin

# Pull da imagem
docker pull ghcr.io/[seu-usuario]/consulta-creditos-dotnet-6-backend:latest
```

#### Usar em docker-compose

```yaml
services:
  api:
    image: ghcr.io/[seu-usuario]/consulta-creditos-dotnet-6-backend:latest
    # ... resto da configuração
```

### Permissões Necessárias

O workflow usa automaticamente o `GITHUB_TOKEN` que já tem permissões para:
- `contents: read` - Ler o repositório
- `packages: write` - Publicar no GHCR

**Não é necessário configurar secrets adicionais!**

### Cache

O pipeline utiliza GitHub Actions Cache para acelerar builds subsequentes:
- Cache de layers Docker
- Reduz tempo de build em ~50-70%

### Tags Geradas

Dependendo do trigger, diferentes tags são geradas:

| Trigger | Tags Geradas |
|---------|--------------|
| Push em `main` | `main`, `latest` |
| Tag `v1.2.3` | `v1.2.3`, `1.2.3`, `1.2`, `1`, `latest` |
| PR | Apenas build (sem push) |
| Manual com tag | Tag especificada |

### Troubleshooting

#### Erro de permissão
- Verifique se o repositório está público ou você tem acesso
- Verifique se o `GITHUB_TOKEN` tem permissões de escrita em packages

#### Imagem não aparece no GHCR
- Verifique se o workflow foi executado com sucesso
- Verifique os logs do workflow em **Actions**
- Certifique-se de que não é um PR (PRs não fazem push)

#### Build falha
- Verifique se o Dockerfile está correto
- Verifique se todas as dependências estão no repositório
- Verifique os logs do workflow para detalhes

### Exemplo de Uso Completo

```bash
# 1. Fazer alterações e commit
git add .
git commit -m "feat: nova funcionalidade"

# 2. Push para main (dispara o pipeline)
git push origin main

# 3. Aguardar pipeline completar (ver em Actions)

# 4. Usar a imagem em outro ambiente
docker pull ghcr.io/[seu-usuario]/consulta-creditos-dotnet-6-backend:latest
docker run -p 8080:8080 ghcr.io/[seu-usuario]/consulta-creditos-dotnet-6-backend:latest
```

