# TimeWise API

O TimeWise é uma aplicação que permite que usuários criem e gerenciem hábitos como pausas, postura e hidratação durante o trabalho.

## ✍️ Integrantes

- [Letícia Zago de Souza](https://www.linkedin.com/in/letícia-zago-de-souza)
- [Ana Carolina Reis Santana](https://www.linkedin.com/in/ana-carolina-santana-9a0a78232)
- [Pedro Henrique Mendonça de Novais](https://www.linkedin.com/in/pedroonovais)

## 🚀 Tecnologias

- **.NET 8.0**
- **Entity Framework Core** com Oracle Database
- **OpenTelemetry** para Tracing
- **xUnit** para testes automatizados
- **Swagger/OpenAPI** para documentação

## 📋 Estrutura do Projeto

```
TimeWise/
├── TimeWise.API/          # Camada de apresentação (Controllers, DTOs)
├── TimeWise.Core/         # Camada de domínio (Models, Interfaces)
├── TimeWise.Service/      # Camada de serviços (Lógica de negócio)
├── TimeWise.Data/         # Camada de dados (DbContext, Repositories, Migrations)
└── TimeWise.Tests/        # Testes automatizados (Unitários e Integração)
```

## 🔄 Versionamento da API

A API utiliza versionamento por URL para garantir compatibilidade e evolução controlada. As versões são estruturadas da seguinte forma:

### Versões Disponíveis

- **v1.0** (Atual): Primeira versão estável da API
  - Endpoint base: `/api/v1/`

### Como Usar o Versionamento

#### Versão Atual (v1.0)

Todas as rotas seguem o padrão `/api/v1/{controller}`:

```http
GET    /api/v1/Habits
GET    /api/v1/Habits/{id}
POST   /api/v1/Habits
PUT    /api/v1/Habits/{id}
DELETE /api/v1/Habits/{id}
```

#### Exemplo de Requisição

```bash
# Criar um hábito
POST /api/v1/Habits
Content-Type: application/json

{
  "usuarioId": "123e4567-e89b-12d3-a456-426614174000",
  "titulo": "Pausa para alongamento",
  "descricao": "Levantar e fazer alongamento a cada 2 horas",
  "tipo": "PAUSA"
}
```

#### Versões Futuras (v2.0)

Quando uma nova versão for necessária, será criada uma nova rota:

```http
GET    /api/v2/Habits
POST   /api/v2/Habits
```

A versão anterior (v1.0) continuará funcionando para garantir compatibilidade com clientes existentes.

### Configuração do Versionamento

O versionamento é configurado no `Program.cs`:

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
```

**Comportamento:**
- `AssumeDefaultVersionWhenUnspecified = true`: Se nenhuma versão for especificada, usa a versão padrão (v1.0)
- `DefaultApiVersion = new ApiVersion(1, 0)`: Define v1.0 como versão padrão
- `ReportApiVersions = true`: Informa quais versões estão disponíveis nos headers da resposta
- `UrlSegmentApiVersionReader`: Lê a versão diretamente da URL (`/api/v1/...`)

### Headers de Resposta

As respostas incluem headers informando as versões disponíveis:

```
api-supported-versions: 1.0
api-deprecated-versions: (nenhuma no momento)
```

## 🗄️ Banco de Dados

### Oracle Database

A aplicação utiliza **Oracle Database** como banco de dados relacional, configurado através do Entity Framework Core.

### Migrations

O projeto utiliza **Entity Framework Core Migrations** para gerenciar o esquema do banco de dados.

#### Criar uma Nova Migration

```bash
dotnet ef migrations add NomeDaMigration --project TimeWise.Data --startup-project TimeWise.API
```

#### Aplicar Migrations ao Banco de Dados

```bash
dotnet ef database update --project TimeWise.Data --startup-project TimeWise.API
```

#### Reverter uma Migration

```bash
dotnet ef database update NomeDaMigrationAnterior --project TimeWise.Data --startup-project TimeWise.API
```

### Connection String

Configure a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=usuario;Password=senha;Data Source=//host:porta/SID;"
  }
}
```

## 🧪 Testes

O projeto inclui testes automatizados usando **xUnit**:

### Testes de Integração

Localizados em `TimeWise.Tests/HabitsIntegrationTests.cs`, testam o comportamento completo da API:

- Criação de hábitos
- Consulta paginada
- Validação de status codes
- Verificação de headers

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --verbosity normal

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~HabitsIntegrationTests"
```

## 📡 Endpoints da API

### Habits

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/Habits` | Lista hábitos com paginação |
| GET | `/api/v1/Habits/{id}` | Obtém um hábito específico |
| POST | `/api/v1/Habits` | Cria um novo hábito |
| PUT | `/api/v1/Habits/{id}` | Atualiza um hábito existente |
| DELETE | `/api/v1/Habits/{id}` | Remove um hábito |

### Parâmetros de Paginação

- `pageNumber` (padrão: 1)
- `pageSize` (padrão: 10, máximo: 50)
- `usuarioId` (opcional, filtra por usuário)

### Exemplo de Resposta com Paginação

```json
{
  "items": [...],
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

Headers adicionais:
- `X-Total-Count`: Total de registros

## 🔗 HATEOAS

A API implementa HATEOAS (Hypermedia as the Engine of Application State), fornecendo links relacionados em cada resposta:

```json
{
  "id": "...",
  "titulo": "...",
  "links": [
    {
      "rel": "self",
      "href": "/api/v1/Habits/{id}",
      "method": "GET"
    },
    {
      "rel": "update",
      "href": "/api/v1/Habits/{id}",
      "method": "PUT"
    },
    {
      "rel": "delete",
      "href": "/api/v1/Habits/{id}",
      "method": "DELETE"
    }
  ]
}
```

## 🏥 Health Check

Endpoint de verificação de saúde da aplicação:

```http
GET /health
```

Verifica:
- Conectividade com o banco de dados
- Status geral da aplicação

## 📊 Observabilidade

### Logging

A aplicação utiliza o sistema de logging padrão do .NET (`ILogger`), configurável em `appsettings.json`.

### Tracing

Implementado com **OpenTelemetry** para rastreamento distribuído:
- Instrumentação automática do ASP.NET Core
- Exportação para console (desenvolvimento)

## 🚀 Como Executar

1. **Clone o repositório**
2. **Configure a connection string** no `appsettings.json`
3. **Aplique as migrations**:
   ```bash
   dotnet ef database update --project TimeWise.Data --startup-project TimeWise.API
   ```
4. **Execute a aplicação**:
   ```bash
   cd TimeWise.API
   dotnet run
   ```
5. **Acesse o Swagger**: `https://localhost:XXXX/swagger`
