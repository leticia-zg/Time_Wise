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

A API utiliza **versionamento por URL** para garantir compatibilidade e evolução controlada. O sistema permite que múltiplas versões coexistam, garantindo que clientes existentes continuem funcionando enquanto novas funcionalidades são introduzidas.

### Versões Disponíveis

A API atualmente suporta **duas versões**:

- **v1.0**: Primeira versão estável da API
  - Endpoint base: `/api/v1/`
  - Status: ✅ Ativa e suportada

- **v2.0**: Segunda versão da API
  - Endpoint base: `/api/v2/`
  - Status: ✅ Ativa e suportada

### Estrutura de Rotas

Todas as rotas seguem o padrão `/api/v{versão}/{controller}`:

#### Versão 1.0

```http
GET    /api/v1/Habits
GET    /api/v1/Habits/{id}
POST   /api/v1/Habits
PUT    /api/v1/Habits/{id}
DELETE /api/v1/Habits/{id}
```

#### Versão 2.0

```http
GET    /api/v2/Habits
GET    /api/v2/Habits/{id}
POST   /api/v2/Habits
PUT    /api/v2/Habits/{id}
DELETE /api/v2/Habits/{id}
```

### Exemplos de Requisição

#### Criar um hábito na v1.0

```bash
POST /api/v1/Habits
Content-Type: application/json

{
  "usuarioId": "123e4567-e89b-12d3-a456-426614174000",
  "titulo": "Pausa para alongamento",
  "descricao": "Levantar e fazer alongamento a cada 2 horas",
  "tipo": "PAUSA"
}
```

#### Criar um hábito na v2.0

```bash
POST /api/v2/Habits
Content-Type: application/json

{
  "usuarioId": "123e4567-e89b-12d3-a456-426614174000",
  "titulo": "Pausa para alongamento",
  "descricao": "Levantar e fazer alongamento a cada 2 horas",
  "tipo": "PAUSA"
}
```

### Organização do Código

Os controllers são organizados por namespace e diretório para facilitar a manutenção:

```
TimeWise.API/
└── Controllers/
    ├── HabitsController.cs          # v1.0 (namespace: Controllers.v1)
    └── v2/
        └── HabitsController.cs      # v2.0 (namespace: Controllers.v2)
```

Cada versão possui seu próprio controller, permitindo evoluções independentes sem quebrar compatibilidade com versões anteriores.

### Configuração do Versionamento

O versionamento é configurado no `Program.cs`:

```csharp
// API Versioning para Swagger
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
```

**Comportamento:**
- `AssumeDefaultVersionWhenUnspecified = true`: Se nenhuma versão for especificada na URL, usa a versão padrão (v1.0)
- `DefaultApiVersion = new ApiVersion(1, 0)`: Define v1.0 como versão padrão
- `ReportApiVersions = true`: Informa quais versões estão disponíveis nos headers da resposta HTTP
- `UrlSegmentApiVersionReader`: Lê a versão diretamente do segmento da URL (`/api/v1/...` ou `/api/v2/...`)

### Documentação Swagger

O Swagger está configurado para exibir ambas as versões:

- **Swagger UI**: Acesse `/swagger` para ver a documentação interativa
- **Seleção de Versão**: Use o dropdown no topo do Swagger UI para alternar entre v1.0 e v2.0
- **Endpoints Separados**: Cada versão possui sua própria documentação OpenAPI

### Headers de Resposta

As respostas HTTP incluem headers informando as versões disponíveis:

```
api-supported-versions: 1.0, 2.0
api-deprecated-versions: (nenhuma no momento)
```

### Política de Compatibilidade

- ✅ **Versões antigas são mantidas**: A v1.0 continuará funcionando mesmo após o lançamento de novas versões
- ✅ **Evolução independente**: Cada versão pode evoluir sem afetar as outras
- ✅ **Migração gradual**: Clientes podem migrar para novas versões no seu próprio ritmo
- ✅ **Documentação separada**: Cada versão possui documentação própria no Swagger

### Quando Criar uma Nova Versão

Uma nova versão deve ser criada quando:

- Mudanças que quebram compatibilidade com versões anteriores
- Alterações significativas na estrutura de dados (DTOs)
- Mudanças em comportamentos existentes que podem afetar clientes
- Introdução de novos recursos que alteram o contrato da API

### Migração entre Versões

Para migrar de v1.0 para v2.0:

1. Atualize a URL base de `/api/v1/` para `/api/v2/`
2. Verifique se há mudanças nos DTOs ou comportamentos
3. Teste todas as funcionalidades utilizadas
4. Atualize a documentação interna se necessário

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

#### Versão 1.0

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/Habits` | Lista hábitos com paginação |
| GET | `/api/v1/Habits/{id}` | Obtém um hábito específico |
| POST | `/api/v1/Habits` | Cria um novo hábito |
| PUT | `/api/v1/Habits/{id}` | Atualiza um hábito existente |
| DELETE | `/api/v1/Habits/{id}` | Remove um hábito |

#### Versão 2.0

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v2/Habits` | Lista hábitos com paginação |
| GET | `/api/v2/Habits/{id}` | Obtém um hábito específico |
| POST | `/api/v2/Habits` | Cria um novo hábito |
| PUT | `/api/v2/Habits/{id}` | Atualiza um hábito existente |
| DELETE | `/api/v2/Habits/{id}` | Remove um hábito |

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
