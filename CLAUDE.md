# CLAUDE.md — Manual operacional do PortfolioApi

> Guia prático para contribuir neste repositório. Complementa (não substitui) o
> `DDD_STANDARDS.md`, que é o documento aspiracional/histórico da arquitetura
> FreteFy. Este arquivo é **a regra concreta para este projeto aqui**: como está
> organizado hoje, o que fazer e o que não fazer.
>
> Se houver conflito entre `CLAUDE.md` e `DDD_STANDARDS.md`, **vale o CLAUDE.md**
> (o standards é referência; este é a implementação acordada).

---

## 1. Shape da solução

```
Portfolio.Domain          → Entidades, interfaces, Domain Services
Portfolio.Application     → DTOs, Validators, Mappings, AppServices
Portfolio.Infrastructure  → EF Core, Repositories, UnitOfWork, Mappings (EF)
PortfolioApi              → Controllers, Middlewares, Swagger filters, Program.cs
Portfolio.Tests           → xUnit + Moq + FluentAssertions
```

Dependências permitidas:
```
PortfolioApi ──► Application ──► Domain
     │               ▲              ▲
     └──► Infrastructure ───────────┘
```

**Proibido:**
- Domain referenciar Application ou Infrastructure.
- Application referenciar Infrastructure (depende só das abstrações no Domain).
- Domain referenciar pacotes externos além de `Microsoft.Extensions.DependencyInjection.Abstractions` (apenas para o `DomainContainer.AddDomain()`).

---

## 2. Fluxo do request (padrão obrigatório)

```
HTTP → Controller (herda BaseController<TEntity, TDto>)
        │
        ▼
   IXxxAppService (herda AppService<TEntity, TDto>)
        │  valida DTO, mapeia DTO↔Entity, chama UoW.Begin()/Confirm()
        ▼
   IXxxService (herda DomainService<T>)
        │  lugar das invariantes e regras de negócio cross-entity
        ▼
   IXxxRepository (herda GenericRepository<T>)
        │  Add/Update/Remove no change tracker (NÃO persiste), Get* no DbSet
        ▼
   AppDbContext (EF Core)
        │
   IUnitOfWork.Confirm() → SaveChangesAsync
```

**Quem persiste**: APENAS `UnitOfWork.Confirm()`. Nunca o Repository, nunca o DbContext chamado direto da Application.

---

## 3. Regras rígidas

### 3.1 NÃO usar

- **MediatR / CQRS**: removemos intencionalmente. Controller não chama `IMediator`. Sem Commands, Queries, Handlers. O fluxo é por herança genérica.
- **`SaveChanges` fora do `UnitOfWork`**: `GenericRepository` só marca estado (`Add`/`Update`/`Remove`). Quem salva é o UoW.
- **Controller chamando Repository ou DbContext**: atravessa camadas.
- **AppService chamando outro AppService**: se precisar compor regras, use Domain Service.
- **Domain Service chamando AppService**: nunca — a dependência é sempre para baixo.
- **Result<T> / Error pattern**: removemos. Erros são `EntityValidationException` / `EntityNotFoundException`; o `GlobalExceptionHandler` traduz em HTTP (400 / 404 / 500).

### 3.2 SEMPRE usar

- **Id gerado no servidor**: `AppService.CreateAsync` faz `entity.Id = Guid.NewGuid()` incondicionalmente. Id vindo do body é ignorado.
- **Id da URL é autoritativo no PUT**: `AppService.UpdateAsync(Guid id, TDto dto)` → `existente.Id = id`, independente do que veio no body.
- **Métodos `virtual`** nas classes base (`AppService`, `DomainService`, `GenericRepository`, `BaseController`) para permitir `override`.
- **FluentValidation** para validação de DTO. O `AppService` recebe `IEnumerable<IValidator<TDto>>` e aplica se houver validator registrado.
- **Mapster (`IRegister`)** para mapping DTO↔Entity, com `.Ignore(dest => dest.Id)` na direção DTO→Entity como defesa em profundidade.

---

## 4. Hierarquia de herança (por camada)

### Domain
```csharp
IRepository<T>                      // interface genérica
  └─ IXxxRepository : IRepository<Xxx>   // interface específica por agregado

IDomainService<T>                   // interface genérica
DomainService<T> : IDomainService<T>   // base concreta com CRUD básico (virtual)
  └─ IXxxService : IDomainService<Xxx>
  └─ XxxService : DomainService<Xxx>, IXxxService
```

### Application
```csharp
IAppService<TEntity, TDto>          // interface genérica
AppService<TEntity, TDto> : IAppService<TEntity, TDto>
  └─ IXxxAppService : IAppService<Xxx, XxxDto>
  └─ XxxAppService : AppService<Xxx, XxxDto>, IXxxAppService
```

### Infrastructure
```csharp
GenericRepository<T> : IRepository<T>
  └─ XxxRepository : GenericRepository<Xxx>, IXxxRepository
```

### Api
```csharp
BaseController<TEntity, TDto>       // abstrata, 5 rotas CRUD
  └─ XxxController : BaseController<Xxx, XxxDto>
```

---

## 5. Checklist para adicionar nova entidade (`Foo`)

Siga esta ordem. Cada subclasse fica idealmente só com o construtor.

**Domain:**
1. `Portfolio.Domain/Entities/Foo.cs` → herda `Entity` (ou `EntityAudited` se precisar auditoria).
2. `Portfolio.Domain/Interface/IFooRepository.cs` → `interface IFooRepository : IRepository<Foo> {}`.
3. `Portfolio.Domain/Interface/IFooService.cs` → `interface IFooService : IDomainService<Foo> {}`.
4. `Portfolio.Domain/Services/FooService.cs` → `class FooService : DomainService<Foo>, IFooService { ctor(IFooRepository) : base(repo) {} }`.
5. Registrar em `DomainContainer.AddDomain()`: `services.AddScoped<IFooService, FooService>();`.

**Application:**
6. `Portfolio.Application/DTOs/Foos/FooDto.cs` → property-style record, `[ReadOnly(true)]` em `Id`.
7. `Portfolio.Application/Mappings/FooMapping.cs` → `IRegister` com `NewConfig<Foo, FooDto>()` e `NewConfig<FooDto, Foo>().Ignore(d => d.Id)`.
8. `Portfolio.Application/Validators/Foos/FooDtoValidator.cs` → `AbstractValidator<FooDto>` com as regras do DTO.
9. `Portfolio.Application/Interface/IFoosAppService.cs` → `interface IFoosAppService : IAppService<Foo, FooDto> {}`.
10. `Portfolio.Application/Services/FoosAppService.cs` → `class FoosAppService : AppService<Foo, FooDto>, IFoosAppService { ctor(IFooService, IUnitOfWork, IMapper, IEnumerable<IValidator<FooDto>>) : base(...) {} }`.
11. Registrar em `Application/DependencyInjection.cs`: `services.AddScoped<IFoosAppService, FoosAppService>();`.

**Infrastructure:**
12. `Portfolio.Infrastructure/Data/Map/FooMap.cs` → `IEntityTypeConfiguration<Foo>`.
13. `Portfolio.Infrastructure/Repositories/FooRepository.cs` → `class FooRepository : GenericRepository<Foo>, IFooRepository { ctor(AppDbContext) : base(ctx) {} }`.
14. Adicionar `DbSet<Foo> Foos => Set<Foo>();` em `AppDbContext`.
15. Registrar em `Infrastructure/DependencyInjection.cs`: `services.AddScoped<IFooRepository, FooRepository>();`.

**Api:**
16. `PortfolioApi/Controllers/FoosController.cs`:
    ```csharp
    public sealed class FoosController : BaseController<Foo, FooDto>
    {
        public FoosController(IFoosAppService service) : base(service) { }
    }
    ```

**Banco:**
17. `dotnet ef migrations add AddFoo -p Portfolio.Infrastructure -s PortfolioApi`.
18. `dotnet ef database update -p Portfolio.Infrastructure -s PortfolioApi`.

**Testes (opcional mas recomendado):**
19. `Portfolio.Tests/Domain/FooServiceTests.cs` se houver lógica em `FooService`.
20. `Portfolio.Tests/Application/Foos/FoosAppServiceTests.cs` se houver overrides.
21. `Portfolio.Tests/Application/Foos/FooDtoValidatorTests.cs` para as regras do validator.

Resultado: 5 rotas CRUD funcionando (`GET /api/foos`, `GET /api/foos/{id}`, `POST /api/foos`, `PUT /api/foos/{id}`, `DELETE /api/foos/{id}`) sem código repetido no controller.

---

## 6. Convenções

### 6.1 DTO

```csharp
public sealed record FooDto
{
    [ReadOnly(true)]
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;
    // ... demais propriedades com get/init
}
```

- Sempre **property-style record** (não positional), para permitir atributos em propriedades.
- `Id` sempre `[ReadOnly(true)]` — o Swagger oculta no request body via filter.
- `string` inicializado com `string.Empty` (evita `null` com `<Nullable>enable</Nullable>`).
- Um DTO único por entidade (não criar `CreateFooDto`, `UpdateFooDto` separados).

### 6.2 Swagger

Dois filters em `PortfolioApi/Swagger/` registrados em `Program.cs`:
- `ApplyReadOnlyAttributeSchemaFilter` → lê `[ReadOnly(true)]` via reflection e seta `OpenApiSchema.ReadOnly = true`.
- `HideReadOnlyInRequestBodyFilter` → para cada `RequestBody`, substitui o schema por `{Nome}Input` sem as props readOnly.

Efeito: POST/PUT mostram só os campos de entrada; GET retorna todos.

### 6.3 Rotas (herdadas do `BaseController`)

| Verbo | Rota | Body | Retorno |
|---|---|---|---|
| GET | `/api/{controller}` | — | `IEnumerable<TDto>` |
| GET | `/api/{controller}/{id:guid}` | — | `TDto` ou 404 |
| POST | `/api/{controller}` | `TDto` (sem Id) | `TDto` (com Id gerado) |
| PUT | `/api/{controller}/{id:guid}` | `TDto` | `TDto` atualizado |
| DELETE | `/api/{controller}/{id:guid}` | — | 204 NoContent |

Para adicionar rotas extras, criar método no controller específico **sem** mexer no `BaseController`.

### 6.4 Exceções

- `EntityNotFoundException` (Application) → GlobalExceptionHandler devolve **404**.
- `EntityValidationException` (Application) → GlobalExceptionHandler devolve **400** com lista de erros.
- Qualquer outra → **500** com detalhe (stack trace só em Development).

### 6.5 Auditoria (automática)

Se uma entidade precisa dos campos `CreationTime`, `CreationUserId`, `ModificationTime`, `ModificationUserId`, basta herdar de `EntityAudited` em vez de `Entity`. O `UnitOfWork.Confirm()` preenche tudo automaticamente via `ChangeTracker.Entries<IMustAudited>()`:

- **Added** → `CreationTime = DateTime.UtcNow`, `CreationUserId = IUserResolver.GetCurrentUserId()`.
- **Modified** → `ModificationTime = DateTime.UtcNow`, `ModificationUserId = ...`; `CreationTime`/`CreationUserId` são explicitamente marcados como `IsModified = false` (preservados).

Enquanto não houver autenticação, `IUserResolver` é resolvido por `CurrentUserResolver` que retorna `Guid.Empty`. Quando plugar JWT, trocar a implementação por uma que leia o claim do `HttpContext` — o UoW não muda.

### 6.6 Naming

- **Português** para entidades, DTOs, métodos de negócio (`Mensagem`, `Ativar`, `Recuperar`).
- **Inglês** para infraestrutura técnica (`Repository`, `Service`, `DbContext`, `UnitOfWork`).
- Prefixo `I` em interfaces.
- Sufixo `Dto` em DTOs, `Map` em mappings de EF, `Validator` em validators.
- Tabelas no banco em **snake_case minúsculo plural** (`mensagens`, `usuarios`).

---

## 7. Comandos úteis

```bash
# Build e testes
dotnet build PortfolioApi.slnx
dotnet test Portfolio.Tests/Portfolio.Tests.csproj

# Migrations (rodado do root da solution)
dotnet ef migrations add <Nome> -p Portfolio.Infrastructure -s PortfolioApi
dotnet ef database update     -p Portfolio.Infrastructure -s PortfolioApi

# Rodar a API
dotnet run --project PortfolioApi/PortfolioApi.csproj --launch-profile http
# → http://localhost:5286/swagger
```

---

## 8. Débitos técnicos conhecidos

- Credenciais do MySQL estão em `appsettings.Development.json` (ver DDD_STANDARDS §8.8) — mover para User Secrets / Azure Key Vault antes de produção.
- Sem autenticação / autorização configurada. `CurrentUserResolver` sempre devolve `Guid.Empty`; quando plugar JWT, substituir por implementação que lê claim do `HttpContext`.
- Sem rate limiting, CORS restritivo, security headers.
- Testes de Infrastructure e Controllers inexistentes (requerem `EF InMemory` / `WebApplicationFactory`).
