# Testes Unitários - Controle de Gastos Residenciais

## 📋 Visão Geral

Este projeto contém testes unitários abrangentes para o sistema de Controle de Gastos Residenciais, cobrindo as camadas **Domain**, **Application** e suas regras de negócio.

## 🧪 Frameworks e Ferramentas Utilizados

- **xUnit**: Framework de testes para .NET
- **Moq**: Biblioteca para criação de mocks e stubs
- **FluentAssertions**: Biblioteca para asserções mais legíveis e expressivas
- **Microsoft.Extensions.Logging.Abstractions**: Para mock de logging

## 📦 Instalação

```bash
dotnet restore
dotnet build
```

## ▶️ Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com saída detalhada
dotnet test --verbosity detailed

# Executar testes com cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Cobertura de Testes

### Domain Layer

#### Entidades (Domain/Entities)
- ✅ **PessoaTests** (3 testes)
  - Validação de menor de idade em diferentes faixas etárias
  - Inicialização correta de coleções
  - Definição de propriedades

- ✅ **CategoriaTests** (6 testes)
  - Validação de finalidade (Despesa, Receita, Ambas)
  - Método `AceitaTipo()` para diferentes cenários
  - Inicialização de coleções
  - Definição de propriedades

- ✅ **TransacaoTests** (3 testes)
  - Definição de propriedades
  - Valores padrão
  - Relacionamentos com outras entidades

#### Serviços de Domínio (Domain/Services)
- ✅ **TransacaoServiceTests** (7 testes)
  - Validação de categoria incompatível com tipo de transação
  - Validação de menor de idade criando receita (deve falhar)
  - Validação de menor de idade criando despesa (deve passar)
  - Validação de maior de idade criando receita (deve passar)
  - Validação de categoria "Ambas" com diferentes tipos de transação

### Application Layer

#### Serviços de Aplicação (Application/Services)

- ✅ **CategoriaAppServiceTests** (7 testes)
  - Obter entidade por ID (sucesso e falha)
  - Listar todas as categorias com totais
  - Obter DTO por ID
  - Criar nova categoria
  - Atualizar categoria existente
  - Deletar categoria

- ✅ **PessoaAppServiceTests** (6 testes)
  - Obter entidade por ID (sucesso e falha)
  - Criar nova pessoa
  - Atualizar pessoa existente
  - Deletar pessoa
  - Obter totais por pessoa

- ✅ **TransacaoAppServiceTests** (13 testes)
  - Obter todas as transações
  - Obter transação por ID (sucesso e falha)
  - Criar transação válida
  - Validação de descrição vazia
  - Validação de valor negativo
  - Atualizar transação
  - Deletar transação (sucesso e falha)

## 📈 Estatísticas

- **Total de Testes**: 45
- **Testes Passando**: 45 ✅
- **Taxa de Sucesso**: 100%

## 🏗️ Estrutura de Diretórios

```
ControleGastosResiduais.Tests/
├── Domain/
│   ├── Entities/
│   │   ├── CategoriaTests.cs
│   │   ├── PessoaTests.cs
│   │   └── TransacaoTests.cs
│   └── Services/
│       └── TransacaoServiceTests.cs
├── Application/
│   └── Services/
│       ├── CategoriaAppServiceTests.cs
│       ├── PessoaAppServiceTests.cs
│       └── TransacaoAppServiceTests.cs
└── README.md
```

## 🎯 Padrões de Teste

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA:
```csharp
[Fact]
public async Task MetodoTestado_Condicao_ResultadoEsperado()
{
    // Arrange - Configuração
    var entidade = new Entidade { /* ... */ };
    
    // Act - Execução
    var resultado = await _service.MetodoTestado(entidade);
    
    // Assert - Verificação
    resultado.Should().NotBeNull();
    resultado.Propriedade.Should().Be(valorEsperado);
}
```

### Nomenclatura de Testes
- `MetodoTestado_Condicao_ResultadoEsperado`
- Exemplo: `CriarAsync_TransacaoValida_DeveCriarERetornarDto`

### Uso de Mocks
- Repositórios são mockados para isolar a lógica de negócio
- Loggers são mockados para evitar dependências externas
- Serviços de domínio são instanciados quando possível (testes de integração de unidade)

### FluentAssertions
Utilizado para asserções mais legíveis:
```csharp
resultado.Should().NotBeNull();
resultado.Descricao.Should().Be("Valor esperado");
resultado.Idade.Should().BeGreaterThan(18);
exception.Should().Throw<ValidacaoNegocioException>()
    .WithMessage("*mensagem esperada*");
```

## 🔍 Cenários Testados

### Validações de Negócio
- ✅ Menor de idade não pode criar receitas
- ✅ Categoria deve aceitar apenas tipos compatíveis
- ✅ Descrição obrigatória para transações e categorias
- ✅ Valor deve ser positivo
- ✅ Categoria com transações não pode ser deletada

### Operações CRUD
- ✅ Criação de entidades com validações
- ✅ Atualização de entidades existentes
- ✅ Deleção com verificações de integridade
- ✅ Consultas com e sem relacionamentos

### Tratamento de Erros
- ✅ Entidade não encontrada lança exceção apropriada
- ✅ Validações de negócio lançam `ValidacaoNegocioException`
- ✅ Campos obrigatórios validados

## 🚀 Próximos Passos

- [ ] Adicionar testes de integração com banco de dados em memória
- [ ] Implementar testes de controllers (API)
- [ ] Configurar análise de cobertura de código
- [ ] Adicionar testes de performance
- [ ] Implementar testes end-to-end

## 📝 Notas Importantes

1. **Mocks vs Instâncias Reais**: 
   - Repositórios são sempre mockados
   - Serviços de domínio usam instâncias reais quando possível
   - AppServices podem ser instâncias reais para testes mais integrados

2. **Async/Await**: 
   - Todos os testes de serviços são assíncronos
   - Use `async Task` e `await` corretamente

3. **Verificação de Mocks**:
   - Use `Verify()` para garantir que métodos foram chamados
   - Especifique `Times.Once`, `Times.Never`, etc.

## 📚 Referências

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/)
