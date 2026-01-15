# Sistema de Controle de Gastos Residenciais

## 📋 Visão Geral

Sistema de controle de gastos residenciais desenvolvido em .NET 10 Web API com arquitetura Clean Architecture simplificada.

## 🏗️ Arquitetura

O projeto utiliza **Clean Architecture** com separação clara de responsabilidades:

```
┌─────────────┐
│     API     │  ← Controllers, HTTP, Validation
├─────────────┤
│ Application │  ← Services (orquestração, casos de uso)
├─────────────┤
│   Domain    │  ← Entities, Business Rules, Interfaces
├─────────────┤
│Infrastructure│ ← EF Core, DbContext, Repositories
└─────────────┘
```

**Fluxo de requisição:**
```
HTTP Request → Controller → Application Service → Domain/Repository → Database
```

## 📁 Estrutura do Projeto

```
Controle de Gastos Residenciais/
├── architecture/                    # Documentação arquitetural
│   ├── plan.md                      # Plano de arquitetura
│   ├── model.md                     # Modelo de domínio
│   └── persistence.md               # Estratégia de persistência
├── Application/                     # Camada de aplicação
│   └── Services/                    # Services de orquestração
│       ├── PessoaAppService.cs
│       ├── CategoriaAppService.cs
│       └── TransacaoAppService.cs
├── Domain/                          # Camada de domínio
│   ├── Entities/                    # Entidades (Pessoa, Categoria, Transacao)
│   ├── Enums/                       # Enumerações (TipoTransacao, FinalidadeCategoria)
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Exceptions/                  # Exceções de domínio
│   ├── Interfaces/                  # Contratos de repositórios
│   └── Services/                    # Serviços de domínio (regras de negócio)
├── Infrastructure/                  # Camada de infraestrutura
│   ├── Data/                        # DbContext do EF Core
│   ├── Repositories/                # Implementações de repositórios
│   └── Migrations/                  # Migrations do banco de dados
└── Controllers/                     # API Controllers (camada HTTP)
    ├── PessoasController.cs
    ├── CategoriasController.cs
    └── TransacoesController.cs
```

## 🚀 Como Executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2025+ ou VS Code com extensão C#

### Passos

1. **Restaurar pacotes:**
   ```bash
   dotnet restore
   ```

2. **Aplicar migrations**:
   ```bash
   dotnet ef database update
   ```

3. **Executar a aplicação:**
   ```bash
   dotnet run
   ```

4. **Acessar a API:**
   - Swagger UI: `http://localhost:5000/openapi/v1.json`
   - Base URL: `http://localhost:5000/api`

## 📊 Banco de Dados

- **Provider:** SQLite
- **Arquivo:** `gastosresiduais.db` (raiz do projeto)
- **Migrations:** Localizadas em `Infrastructure/Migrations/`

### Comandos úteis

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations
dotnet ef database update

# Remover última migration
dotnet ef migrations remove
```

## 🔗 Endpoints Disponíveis

### Pessoas

- `GET /api/pessoas` - Listar todas as pessoas
- `GET /api/pessoas/{id}` - Buscar pessoa por ID
- `POST /api/pessoas` - Criar nova pessoa
- `DELETE /api/pessoas/{id}` - Deletar pessoa (e suas transações)
- `GET /api/pessoas/totais` - Totais por pessoa + totais gerais

### Categorias

- `GET /api/categorias` - Listar todas as categorias
- `GET /api/categorias/{id}` - Buscar categoria por ID
- `POST /api/categorias` - Criar nova categoria
- `GET /api/categorias/totais` - Totais por categoria (opcional)

### Transações

- `GET /api/transacoes` - Listar todas as transações
- `GET /api/transacoes/{id}` - Buscar transação por ID
- `POST /api/transacoes` - Criar nova transação
- `GET /api/transacoes/pessoa/{pessoaId}` - Transações de uma pessoa
- `GET /api/transacoes/categoria/{categoriaId}` - Transações de uma categoria

## 📝 Regras de Negócio

### RN01: Validação de Categoria vs Tipo de Transação
- Transações do tipo **Despesa** só podem usar categorias com finalidade `Despesa` ou `Ambas`
- Transações do tipo **Receita** só podem usar categorias com finalidade `Receita` ou `Ambas`
- **Implementação:** `TransacaoService.ValidarTransacao()` (Domain Layer)

### RN02: Menor de Idade Apenas Despesas
- Pessoas com **idade < 18** só podem criar transações do tipo `Despesa`
- **Implementação:** `TransacaoService.ValidarTransacao()` (Domain Layer)

### RN03: Cascade Delete de Transações
- Ao deletar uma **Pessoa**, todas as suas **Transações** são removidas automaticamente
- **Implementação:** EF Core Cascade Delete (Infrastructure Layer)

## 🧪 Testando a API

### Exemplo: Criar Pessoa

```bash
POST /api/pessoas
Content-Type: application/json

{
  "nome": "João Silva",
  "idade": 30
}
```

### Exemplo: Criar Categoria

```bash
POST /api/categorias
Content-Type: application/json

{
  "descricao": "Alimentação",
  "finalidade": 0  // 0=Despesa, 1=Receita, 2=Ambas
}
```

### Exemplo: Criar Transação

```bash
POST /api/transacoes
Content-Type: application/json

{
  "descricao": "Compra no supermercado",
  "valor": 150.00,
  "tipo": 0,  // 0=Despesa, 1=Receita
  "pessoaId": 1,
  "categoriaId": 1
}
```

### Exemplo: Consultar Totais por Pessoa

```bash
GET /api/pessoas/totais
```

Resposta:
```json
{
  "pessoas": [
    {
      "id": 1,
      "nome": "João Silva",
      "totalReceitas": 5000.00,
      "totalDespesas": 3200.00,
      "saldo": 1800.00
    }
  ],
  "totaisGerais": {
    "totalReceitas": 5000.00,
    "totalDespesas": 3200.00,
    "saldoLiquido": 1800.00
  }
}
```

## 🛠️ Tecnologias Utilizadas

- **.NET 10 Web API** - Framework principal
- **Entity Framework Core 10** - ORM para acesso a dados
- **SQLite** - Banco de dados local
- **Clean Architecture** - Separação em camadas (Application, Domain, Infrastructure)
- **Dependency Injection** - Inversão de controle
- **Repository Pattern** - Abstração de persistência
- **Domain Services** - Lógica de negócio
- **Application Services** - Orquestração de casos de uso


