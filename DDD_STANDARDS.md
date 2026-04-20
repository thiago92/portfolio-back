# DDD_STANDARDS — Padrões da Arquitetura FreteFy

> Documento de referência extraído da solução `PG.FreteFy.sln` (.NET Core 3.1).
> Serve de guia para criar uma API nova em **.NET 8** mantendo a mesma filosofia DDD,
> modernizando tecnologias, corrigindo problemas de performance/segurança e preservando
> os padrões de modelagem que já funcionam bem.

---

## Índice

1. [Organização da Solução](#1-organização-da-solução)
2. [Bounded Contexts](#2-bounded-contexts)
3. [Camada de Domínio](#3-camada-de-domínio)
4. [Camada de Aplicação](#4-camada-de-aplicação)
5. [Camada de Infraestrutura](#5-camada-de-infraestrutura)
6. [Camada de Apresentação (WebApi)](#6-camada-de-apresentação-webapi)
7. [Convenções Transversais](#7-convenções-transversais)
8. [Débitos Técnicos e Code Smells Conhecidos](#8-débitos-técnicos-e-code-smells-conhecidos)
9. [Guia de Migração para .NET 8](#9-guia-de-migração-para-net-8)
10. [Checklist para Novo Projeto](#10-checklist-para-novo-projeto)

---

## 1. Organização da Solução

### 1.1 Topologia de projetos

A solução tem **13 projetos**, agrupados em 3 pastas virtuais (`Domain`, `Applications`, `Tests`).

| Pasta virtual | Projeto | Target | Papel |
|---|---|---|---|
| Domain | `PG.FreteFy.Domain` | netstandard2.1 | Entidades, VOs, Enums, Repositórios (interfaces), Services, Validations |
| Domain | `PG.FreteFy.Application` | netstandard2.1 | Orquestração, DTOs, AutoMapper, App Services |
| Domain | `PG.FreteFy.Infra` | netstandard2.1 | EF Core, MongoDB, Service Bus, integrações |
| Domain | `PG.FreteFy.EventBus` | — | Abstração de event bus |
| Domain | `PG.FreteFy.Messaging` | — | Contratos de mensageria |
| Domain | `PG.FreteFy.Trigger` | — | Triggers externos |
| Applications | `PG.FreteFy.WebApi` | netcoreapp3.1 | API REST principal |
| Applications | `PG.FreteFy.WebHook` | — | Webhooks |
| Applications | `PG.Fretefy.PublicApi` | — | API pública |
| Applications | `PG.FreteFy.BackgroundProcessing` | — | Workers |
| Tests | `PG.FreteFy.Tests` / `.UnitTests` | — | Testes |

Referência base: pacote NuGet **`PG.Core.Domain 1.12.0`** — fornece `Entity`, `EntityAudited`, `ValueObject<T>`, `IRepository<T>`, `IUnitOfWork<T>`, `IDomainService<T>`, `IAppService<T>`, `IEventBus`, `ISpecification<T>`, etc. **Todo o novo projeto deve continuar usando esse pacote como núcleo** para preservar a filosofia.

### 1.2 Grafo de dependências (regra de ouro)

```
WebApi  ─────────────►  Application  ─────────►  Domain
   │                         │                     ▲
   └───► Infra ──────────────┘                     │
              └─────────────────────────────────────┘
                        (implementa IRepository, IXxxService)
```

- **Domain** não pode referenciar `Application` nem `Infra`. *(Atualmente esta regra é violada — ver §8.1.)*
- **Application** pode referenciar `Domain` e `EventBus`; **não deve** referenciar `Infra` diretamente, apenas abstrações no Domain.
- **Infra** implementa as interfaces do `Domain`.
- **WebApi** pode referenciar `Application` e `Infra` **apenas** para composição (IoC no Startup).

### 1.3 Estrutura de pastas por camada

```
Domain/
├── Entities/            # Agregados e entidades por bounded context
├── ValueObjects/
├── Enums/
├── Services/            # Domain Services (lógica pura de negócio)
├── Repositories/        # INTERFACES apenas
├── Validations/         # EntityValidation<T> + Specifications/
├── Exceptions/          # Exceções de domínio
├── Filters/             # Critérios de busca (search criteria)
├── Views/               # Marker interfaces e view models leves
├── Extensions/          # Extensions de tipos do domínio
├── Messages/            # .resx localização
├── Constants.cs
├── Utils.cs
└── DomainContainer.cs   # Extension method RegisterDomainServices

Application/
├── Services/            # Application Services (orquestração)
├── Interfaces/
├── DTO/
├── Web/                 # Services/DTOs específicos do canal Web
├── Mobile/              # Services/DTOs específicos do canal Mobile
├── Auxiliaries/         # Lógica de orquestração reutilizável (Distance, etc.)
├── ApplicationContainer.cs
└── ApplicationMapperProfile.cs

Infra/
├── EntityFramework/
│   ├── FreteFyDbContext.cs
│   ├── Mappings/        # IEntityTypeConfiguration<T>, 1 arquivo por entidade
│   ├── Migrations/
│   ├── Repositories/    # Implementações de IRepository
│   ├── UnitOfWork/
│   └── Extensions/      # PropertyHasPhone, IsOwnedEntity, HasJson
├── MongoDb/             # Read models (CQRS-lite)
├── ServiceBus/          # Azure Service Bus
├── MemoryCache/         # Redis
├── Security/
├── Audit/               # Azure Table Storage
├── Storage/             # Blob Storage
├── <Integração>/        # Twilio, SendGrid, MapBox, Firebase, etc.
└── InfraContainer.cs

WebApi/
├── Controllers/         # Agrupados por bounded context
├── Hubs/                # SignalR
├── Consumers/           # MassTransit consumers
├── Models/              # Request/Response shapes específicas da API
├── Infra/               # Filters, Middleware, Resolvers
├── Program.cs
└── Startup.cs
```

---

## 2. Bounded Contexts

A pasta `Domain/Entities/` reflete 14 bounded contexts identificados:

| Bounded Context | Agregado Raiz | Responsabilidade |
|---|---|---|
| Cargas | `Carga` | Carga, atividades, itinerário, oferecimento, totalizadores |
| Motoristas | `Motorista` | Motorista + CNH + vínculos com embarcadoras |
| Veículos | `Veiculo` | Placa, RNTRC, rastreadores |
| Acordos | `Acordos` | Acordos comerciais entre embarcadores/transportadoras |
| Oferecimento | `Oferecimento*` | Oferta e aceite de fretes |
| Monitoramento | `Monitoramento*` | Rastreamento e KPIs operacionais |
| Execução | `Execucao*` | Execução/histórico de operações |
| Check-list | `Checklist` | Conformidade documental |
| CrossDocking | `Remessa` | Consolidação |
| Alertas | `Alerta*` | Regras de alerta e disparos |
| Envolvidos | `Envolvido` | Pessoas/empresas (polimorfismo de stakeholder) |
| Tabela de Fretes | `TabelaFrete` | Precificação |
| Regiões | `Regiao` | Geografia |
| KPI | `Kpi*` | Indicadores agregados |

**Agregados compartilhados**: `Owner` (tenant raiz) e `UnidadeNegocio` aparecem em todos os contextos via multi-tenancy (ver §7.1).

---

## 3. Camada de Domínio

### 3.1 Hierarquia de building blocks

Vinda de `PG.Core.Domain 1.12.0`:

```
Entity                          → Id: Guid
  └─ EntityAudited              → + CreationUserId, CreationTime, ModificationUserId, ModificationTime
       └─ EntityWithOwner       → + OwnerId
            └─ EntityOwnedAudited → implementa IMustHaveOwner + IMustAudited
                 └─ (concretas) → Motorista, Carga, Veiculo, Acordos, …
```

Marker interfaces de infraestrutura de domínio:

- `IMustHaveOwner` — tenant obrigatório
- `IMustHaveBusinessUnit` — unidade de negócio obrigatória
- `IMustAudited` — campos de auditoria preenchidos automaticamente
- `IMayHaveHolding` — holding opcional
- `IView` — marker para entidades que são projeções (MongoDB)

**Padrão para novo projeto**:
- Manter Guid como tipo de Id.
- Manter a hierarquia acima (continuando a usar `PG.Core.Domain` se compatível com net8.0, ou replicando as classes base localmente em um projeto `BuildingBlocks`).
- Introduzir `AggregateRoot : EntityOwnedAudited` com lista de Domain Events (atualmente **não existe** — ver §9.3).

### 3.2 Value Objects

Base: `ValueObject<T>` genérico com igualdade estrutural. Convenção:

- Construtor vazio `protected` (EF) + construtor público que normaliza.
- Propriedades **read-only externamente** (`public string X { get; private set; }`).
- Método `IsValid()` ou propriedade `IsValid` interna.
- Método `Formatar()` quando aplicável.

Inventário atual de VOs (manter esse vocabulário no novo projeto):

`EmailValueObject`, `CPFValueObject`, `CPFCNPJValueObject`, `CNHValueObject`, `TelefoneValueObject`, `PlacaValueObject`, `RNTRCValueObject`, `RGValueObject`, `CidadeValueObject`, `CidadeIBGEValueObject`, `StorageValueObject`, `FiltroLocalidadeValueObject`, `FormacaoLocalidadeValueObject`, `ImportarPedidoValueObject`.

Exemplo canônico (`Domain/ValueObjects/TelefoneValueObject.cs`):

```csharp
public class TelefoneValueObject : ValueObject<TelefoneValueObject>
{
    private static Regex normalizePhoneRegex = new Regex("[^\\d]", RegexOptions.Compiled);
    private readonly int Maxlength = 15;
    private readonly int Minlength = 10;

    protected TelefoneValueObject() { }
    public TelefoneValueObject(string numero) { Numero = Normalize(numero); }

    public string Numero { get; private set; }
    public bool IsValid() => string.IsNullOrEmpty(Numero)
        || (Numero.Length <= Maxlength && Numero.Length >= Minlength);
    public string Formatar() { /* ... */ }
}
```

**No .NET 8**: VOs podem ser `sealed record` com `init` em vez de classe, e `required` nos campos obrigatórios. Mantenha os **mesmos nomes** do domínio atual.

### 3.3 Entidades e Agregados

Convenções observadas e que devem ser preservadas:

- Construtor `protected` vazio (para EF) **e** construtor público com invariantes.
- Propriedades com `private set;` — mutação **somente** por métodos de negócio.
- Value Objects embutidos como propriedades (`Residencia`, `CNH`, `Documento`, `Telefone`).
- Navigation properties `virtual` quando há lazy loading.
- Nomes de métodos em **português** e no infinitivo imperativo (`Ativar()`, `Desativar()`, `IncluirAtividade()`, `RemoverAtividade()`, `InformarUsuario()`, `GerarCodigo()`).

Exemplo (`Domain/Entities/Motoristas/Motorista.cs`):

```csharp
public class Motorista : EntityOwnedAudited, IMustHaveOwner, IMustAudited, IMustHaveBusinessUnit
{
    protected Motorista() { }

    public Motorista(short pais, string cnhNumero, ETipoMotorista tipoMotorista,
                     EVisibilidadeMotorista visibilidade, Envolvido envolvido, Guid? userId = null)
    {
        Residencia = new CidadeValueObject(pais);
        CNH = new CNHValueObject(cnhNumero);
        Envolvido = envolvido;
        TipoMotorista = tipoMotorista;
        if (userId.HasValue) ModificationUserId = userId.Value;
        Ativar();
    }

    public CNHValueObject CNH { get; private set; }
    public ETipoMotorista TipoMotorista { get; private set; }
    public bool Ativo { get; private set; }
    public virtual Envolvido Envolvido { get; private set; }

    public void Ativar()    => Ativo = true;
    public void Desativar() => Ativo = false;
}
```

**Anti-padrões a evitar no novo projeto**:
- Agregados enormes (`Carga` tem >50 propriedades) — quebre em sub-agregados.
- Propriedades com `public set` (`Observacao`, `Email`, `CategoriaCNH` em `Motorista`).
- Colunas "desnormalizadas" dentro do agregado (`AgendamentoDesnormalizacao`, `OrigemDesnormalizacao` em `Carga`) — essas são projeções, pertencem ao Mongo/view.

### 3.4 Enums

Enums tradicionais C# com prefixo `E` e `[Description]`:

```csharp
public enum ESituacaoCarga
{
    [Description("EmAberto")]     Aberta = 1,
    [Description("EmAndamento")]  Andamento = 2,
    [Description("Finalizada")]   Finalizada = 3,
    [Description("Cancelada")]    Cancelada = 98,
    [Description("Desativada")]   Desativada = 99
}
```

**Manter o padrão.** Gaps numéricos (98/99) marcam estados terminais por convenção. Smart Enums (classes) **não são usados** e não precisam ser introduzidos — mas considere-os para enums com comportamento (por exemplo `ESituacaoCarga` poderia expor `PodeTransitarPara(outro)` como método).

### 3.5 Domain Services

Base `DomainService<T, TValidation>` ou `DomainStorageService<T, TValidation>` (do `PG.Core.Domain`). Responsabilidades:

- Validar invariantes **entre entidades** (ex.: "motorista não pode desativar se tem veículo alocado").
- Orquestrar operações que cruzam múltiplos repositórios do mesmo contexto.
- Não chamar outros Application Services.

Exemplo (`Domain/Services/MotoristaService.cs`):

```csharp
public class MotoristaService :
    DomainStorageService<Motorista, MotoristaEntityValidation>,
    IDomainService<Motorista>, IMotoristaService
{
    public void ValidarMotoristaDuplicado(Guid motoristaId, string documento, Guid? ownerId = null)
    {
        var current = ownerId.HasValue && ownerId != Guid.Empty
            ? ownerId.Value
            : _ownerResolver.GetCurrentOwner();
        if (_motoristaRepository.ExistePeloDocumento(motoristaId, documento, current))
            throw new EntityValidationException("O documento informado já está sendo utilizado.");
    }
}
```

### 3.6 Repositórios (interfaces)

Padrão: interface específica por agregado **herda de `IRepository<T>`** genérico.

```csharp
public interface IMotoristaRepository : IRepository<Motorista>
{
    Motorista GetByDocument(string numeroDocumento, Guid? ownerId = null);
    bool ExistePeloDocumento(Guid motoristaId, string numeroDocumento, Guid? ownerId = null);
    IEnumerable<MotoristaQueryDto> RecuperarMotoristasAtivosOuInativos(bool ativos, Guid? cargaOwnerId = null);
    IQueryable<Motorista> SearchByListId(IEnumerable<Guid> ids);
}
```

Convenções:
- Métodos que retornam projeções devem retornar DTOs (`MotoristaQueryDto`), não a entidade inteira.
- `ownerId` opcional como último parâmetro; quando nulo, resolver via `IOwnerResolver`.
- Evite expor `IQueryable` para fora da camada de Infra. *(Atualmente alguns repositórios violam isso — ver §8.6.)*

### 3.7 Validações

Dois mecanismos coexistem:

**A) `EntityValidation<T>` + Specifications** (para invariantes de persistência)

```csharp
public class MotoristaEntityValidation : EntityValidation<Motorista>
{
    public MotoristaEntityValidation(Motorista entity) : base(entity) { }

    public override void Configure(EntityValidationBuilder<Motorista> builder)
    {
        if (Entity.TipoMotorista == ETipoMotorista.Fisica)
            builder.Add<CPFCNPJValueObject, CPFValidSpecs>(
                p => p.Envolvido.Documento, "CPF inválido");

        builder.Add(new StringLimitSpec(15),
            p => p.CNH.Numero,
            "O CNH não pode ter mais que 15 números.");
    }
}
```

**B) Exceções lançadas manualmente nos Domain/Application Services** (para regras de negócio condicionais).

**Recomendação para novo projeto**: manter as duas abordagens, mas adicionar uma `DomainException` base — hoje as exceções herdam diretamente de `Exception`, sem código de erro ou categorização. Ver §9.5.

### 3.8 Exceções de Domínio

Inventário: `EntityValidationException`, `EntityNotFoundException`, `EntityOperationException`, `AccountAlreadyExistsException`, `AccountViolationException`, `BadCredentialsException`, `CargaUnicaRemessaException`, `CyclicDependencyException`, `DuplicatedComponentsException`, `FlowRequestLoteException`, `MissingOwnerException`, `OwnerBlockedException`, `OwnerNotFoundException`, entre outras.

Padrão existente: `[Serializable]` + construtores padrão de `Exception`. **Nenhuma base comum.**

**No novo projeto**:

```csharp
public abstract class DomainException : Exception
{
    public string Code { get; }
    public IReadOnlyDictionary<string, object>? Details { get; }
    protected DomainException(string code, string message,
        IReadOnlyDictionary<string, object>? details = null, Exception? inner = null)
        : base(message, inner) { Code = code; Details = details; }
}
```

### 3.9 Domain Events (ausente hoje)

**Nenhuma implementação de Domain Events foi encontrada** no Domain atual. Eventos circulam via `IEventBus` (MassTransit) disparados da Application — acoplamento mais alto do que o ideal.

Para o novo projeto (ver §9.3), introduza `IDomainEvent` + `AggregateRoot.AddDomainEvent()` + dispatcher no `SaveChanges` do `DbContext`.

---

## 4. Camada de Aplicação

### 4.1 Hierarquia dos Application Services

```
IAppService<T>
     ▲
AppService<T> : EntityAppService<T, Guid>
     ▲
AppService<T, TDto> : AppService<T>
     ▲
XxxAppService (concreto)
```

Base em `Application/Services/EntityAppService.cs` e `DtoAppService.cs`:

```csharp
public class EntityAppService<TEntity, TKey> : ICrudAppService<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
{
    public virtual TKey Create(TEntity entity)
    {
        _entityUow.Begin();
        var db = _entityService.Create(entity);
        _entityUow.Confirm();
        return db.Id;
    }
    // Update / Delete / Get / List similares
}

public class AppService<TEntity, TDto> : AppService<TEntity>, IDtoAppService<TEntity, TDto>
{
    public virtual Guid Create(TDto dto)
        => base.Create(Mapper.Map<TEntity>(dto));
}
```

### 4.2 Unit of Work

Injetado via `IUnitOfWork<TEntity>`. Contratos: `Begin()`, `Confirm()`, `SaveChanges()`, `SaveChangesAsync()`, `BeginTransaction()`, `ConfirmTransaction()`, `RejectTransaction()`.

Quem faz commit: o **Application Service**. Controllers não tocam no UoW.

Exemplo com transação explícita (multi-SaveChanges):

```csharp
_unitOfWork.BeginTransaction();
try
{
    _unitOfWork.Begin();
    var motorista = _accountService.CriarContaFuncionario(...);
    _veiculoService.Update(motorista.Id, motorista);
    _unitOfWork.Confirm();
    _unitOfWork.ConfirmTransaction();
}
catch
{
    _unitOfWork.RejectTransaction();
    throw;
}
```

### 4.3 DTOs

Convenções:

- **Classes** (não records — o projeto está em netstandard2.1).
- Sufixos: `Dto`, `RequestDto`, `ResponseDto`, `ResultDto`, `SearchDto`, `CreateDto`, `UpdateDto`.
- Propriedades com `public get/set`.
- Subpasta por contexto (`DTO/Cargas/`, `DTO/Motoristas/`…).
- `DTO/Values/` contém espelhos dos VOs (`StorageDto`, `CidadeDto`, `EmailDto`, `RGDto`…).

Validação em DTO é **rara** — quase sempre delegada ao Domain. Apenas alguns casos usam `IValidatableObject`:

```csharp
public class CargaRequestVeiculoEMotoristaDto : IValidatableObject
{
    [Range(0, int.MaxValue)] public int Skip { get; set; }
    [Range(1, 50)]           public int Take { get; set; } = 50;

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (string.IsNullOrWhiteSpace(Placa) && string.IsNullOrWhiteSpace(Documento))
            yield return new ValidationResult("Informe a placa ou o documento.");
    }
}
```

**No novo projeto**: use `record` com `required` (ver §9.7) e adote **FluentValidation** como camada de validação do DTO na borda da API.

### 4.4 AutoMapper

- Versão atual: **AutoMapper 8.1.0** (2019 — desatualizado).
- Configuração em `ApplicationMapperProfile.cs` (1712 linhas — monolito).
- Mapeamentos complexos com `.ForMember()` para construir VOs:
  ```csharp
  CreateMap<WebUserCreateDto, User>()
      .ForMember(t => t.Email,     m => m.MapFrom(s => new EmailValueObject(s.Email)))
      .ForMember(t => t.Documento, m => m.MapFrom(s => new CPFCNPJValueObject(s.Documento)))
      .ForMember(t => t.Telefone,  m => m.MapFrom(s => new TelefoneValueObject(s.Telefone)));
  ```
- `.ReverseMap()` é o padrão quando o DTO é simétrico.

**No novo projeto**: atualizar para AutoMapper 13+ ou **Mapperly** (source generator, zero reflection — ganho de performance significativo). Quebrar o profile em múltiplos profiles por bounded context.

### 4.5 Padrão de retorno e erros

- Retorno: **DTO direto** ou `Guid` (para Create/Update).
- Erros: **exceções** (`EntityValidationException`, `EntityNotFoundException`, `AccountViolationException`…). Sem `Result<T>`.
- Capturadas pelo middleware `ValidationExceptionMiddleware` da WebApi e traduzidas em HTTP.

### 4.6 Event Bus

`IEventBus` (wrapper) → **MassTransit 7.3.1** → Azure Service Bus. Uso:

```csharp
EventBus.Send(new CadastrarExecucaoHistorico(...));     // comando
EventBus.Publish(new VeiculoUpdated(id));                // evento
```

### 4.7 Separação Web vs Mobile

Pastas `Application/Web/` e `Application/Mobile/` são **canais de cliente**, não bounded contexts:
- `Web/Services`, `Web/DTOs`, `Web/Interfaces` — formato esperado pelo portal.
- `Mobile/Services`, `Mobile/DTOs`, `Mobile/Interfaces` — formato esperado pelo app.

Ambos consomem os mesmos Domain Services; diferem só no shape de entrada/saída.

### 4.8 IoC

`ApplicationContainer.cs` registra **tudo como `Transient` explicitamente**. Sem auto-registration (Scrutor) — trade-off: mais verboso, mais previsível.

```csharp
services.AddTransient<IVeiculoAppService, VeiculoAppService>();
services.AddTransient<IAppService<OferecimentoConfiguracao>, AppService<OferecimentoConfiguracao>>();
services.AddTransient<IDtoAppService<Regiao, RegiaoDto>, AppService<Regiao, RegiaoDto>>();
```

**No novo projeto**: considerar Scrutor para reduzir boilerplate, mas manter Transient como default. Serviços com estado (cache local, conexões) = Singleton; serviços com request context = Scoped.

---

## 5. Camada de Infraestrutura

### 5.1 Entity Framework Core

**Stack atual (!)**: EF Core **2.2.6** — EOL em dezembro 2022. A migração para EF Core 8 é obrigatória.

`FreteFyDbContext.cs`:
- Recebe `IOwnerResolver` e `IBusinessUnitResolver` no construtor.
- Override de `Add<TEntity>()` para gerar `Guid` quando `UseEntityId = true`.
- Global Query Filters por `OwnerId` e `UnidadeNegocioId` em ~30 entidades (soft-multi-tenant).

```csharp
modelBuilder.Entity<User>()
    .HasQueryFilter(e => e.OwnerId == OwnerId
        && e.Deleted == false
        && e.Tipo != ETipoUsuario.PortalAcompanhamento
        && e.Tipo != ETipoUsuario.PortalAgendamento);
```

### 5.2 Mappings (IEntityTypeConfiguration)

Padrão **1 arquivo por entidade** em `EntityFramework/Mappings/`:

```csharp
public class AccountMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.IsOwnedEntity();                                          // extension custom
        builder.PropertyHasPhone(nameof(User.Telefone), p => p.Telefone); // VO → coluna
        builder.PropertyHasCPFCNPJ(nameof(User.Documento), p => p.Documento);
    }
}
```

Extensions custom para VOs — **manter** esse padrão no novo projeto:
- `IsOwnedEntity()` — soft delete + filtros globais.
- `PropertyHasPhone`, `PropertyHasCPFCNPJ`, `PropertyHasEmail`, etc.
- `HasJson()` — serializa VOs complexos como JSON (em EF8 usa-se `ComplexProperty` ou `OwnsOne`).

### 5.3 Repositórios (implementações)

Base `GenericRepository<T>` (herança de `PG.Core.Domain`) sobrescrita:

```csharp
protected override IQueryable<Veiculo> GetEntityProxy(DbSet<Veiculo> dbSet)
    => dbSet.Include(i => i.EmbarcadorasVinculadas)
            .Include(x => x.Rastreadores).ThenInclude(x => x.Veiculo);
```

**Anti-patterns a NÃO copiar**:

```csharp
// ❌ VeiculoRepository.cs:44  — carrega a tabela inteira antes de filtrar
var qryVeiculosList = DbSet.Include(i => i.Rastreadores).IgnoreQueryFilters().ToList();
```

```csharp
// ❌ Ausência de AsNoTracking() em queries de leitura — change tracker desnecessário
```

### 5.4 Unit of Work

`EntityFramework/UnitOfWork/UnitOfWork.cs`:
- `BeginTransaction()` usa `Stack<IDbContextTransaction>` para transações aninhadas.
- `SaveChanges(Async)` aplica resolvers automaticamente:
  - `OwnerId` em entidades `IMustHaveOwner`
  - `UnidadeNegocioId` em `IMustHaveBusinessUnit`
  - `CreationUserId` / `ModificationTime` em `IMustAudited`

Essa injeção automática de tenant/auditoria é um dos **maiores ativos** da arquitetura — preservar no novo projeto.

### 5.5 Polyglot: SQL + MongoDB (CQRS-lite)

- **SQL Server** (EF) = write + leitura transacional.
- **MongoDB** (`TrackingViewDbContext`, `MongoDbViewRepository<T>`) = read models para consultas agregadas caras (dashboard de cargas, etc.).

Filtros multi-tenant no Mongo também são globais (`OwnerId`, `UnidadeNegocioId`).

Ex.: `CargaMongoRepository` estende `MongoDbViewRepository<Carga>`.

### 5.6 Service Bus

`ServiceBusTopologyBuilder` cria topics/queues/subscriptions dinamicamente via reflexão sobre `[ServiceBusTrigger]`. Produção e consumo roteados por **MassTransit 7.3.1** configurado no `Startup` da WebApi.

### 5.7 Integrações externas

| Categoria | Implementação | Interface |
|---|---|---|
| SMS | Twilio (`TwilioSmsService`) + Comtele fallback | `ISmsService` (Strategy via `SmsServiceConfig`) |
| Email | SendGrid | `IEmailService` |
| Chat | TalkJS | `IChatService` |
| Push | Firebase + Expo fallback | `IPushNotificationService` / `IFirebaseCloudMessaging` |
| Storage | Azure Blob (⚠️ pacote antigo `WindowsAzure.Storage 9.3.3`) | `IBlobStorageService` |
| Geocoding | EasyAddress, MapBox, Pathfind | — |
| PDF | `PDFConverterService` | `IPDFConverterService` |
| Audit | Azure Table Storage | `ISecurityLogger` |
| Cache | Redis (`StackExchange.Redis 2.2.4`) + `EmptyMemoryCache` fallback | `IMemoryCache` (customizado) |

Padrão: **toda integração tem interface no Domain/Application**, implementação na Infra. Configuração via `IOptions<TConfig>` ou classe `ISomethingConfig`.

### 5.8 Segurança

`Security/SecurityLogger.cs` — grava em Azure Table Storage (partição por `OwnerId`, row key por `DateTime.MaxValue.Ticks - now.Ticks` para ordenação DESC).

`Utils.cs` usa `ComputeMD5Hash` e `ComputeSha256Hash` — **MD5 só deve ser usado para checksum, nunca para senha** (não há evidência de uso para credenciais, mas revisar no novo projeto).

### 5.9 IoC

`InfraContainer.cs` — padrão builder para SMS:

```csharp
services.AddSmsServices(config => config.UseTwilio());
services.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));
services.AddTransient(typeof(IRepository<Motorista>), typeof(MotoristaRepository));
services.AddTransient(typeof(IMotoristaRepository),    typeof(MotoristaRepository));
```

---

## 6. Camada de Apresentação (WebApi)

### 6.1 Startup/Program

- `Program.cs` clássico com `WebHost.CreateDefaultBuilder()` + `UseIIS()` + `UseStartup<Startup>()`.
- `Startup.cs` com `ConfigureServices` e `Configure`.
- Configurações por ambiente: Development, Homolog, Tests, Production.

### 6.2 Pipeline

Ordem (não alterar):

```
HealthCheck
  → (Dev) UseDeveloperExceptionPage
  → ValidationExceptionMiddleware (global exception handler)
  → (Prod) UseHsts
  → UseRouting
  → UseCors
  → UseAuthentication (JWT Bearer)
  → UseAuthorization (policy "Bearer" + BusinessUnitAuthorizationFilter global)
  → UseEndpoints (Controllers + 7 Hubs SignalR)
```

### 6.3 Controllers

Convenções:

- Herdam de `ControllerBase` **ou** de `BaseController` customizado (adiciona header `Reason` em `NotFound`).
- Rota: `[Route("api/<recurso>")]`, `[Produces("application/json")]`.
- `[Authorize]` global via `AuthorizeFilter` + `BusinessUnitAuthorizationFilter`.
- **Thin controllers**: injetam App Services e delegam; validação inline mínima.

```csharp
[Route("api/documento")]
public class DocumentoController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        if (id == Guid.Empty) return BadRequest();
        return Ok(_entityAppService.Get(id));
    }
}
```

**Anti-patterns detectados** (evitar no novo):
- `CargaDocumentoController.Patch` faz `try/catch CargaUnicaRemessaException` e chama múltiplos app services no catch — lógica de negócio no controller.
- `OcorrenciaController.ResolverOcorrencia` converte `IFormFile → StorageDto` na controller (vazamento).

### 6.4 Filtros

- **Global**: `BusinessUnitAuthorizationFilter` — valida claims `BusinessUnitId` contra `IBusinessUnitResolver` (exceto para `[AllowAnonymous]` e roles `Motorista`/`PortalAgendamento`/`PortalAcompanhamento`).
- **Por atributo**: `[DenyLicenseFreeFilter]` — bloqueia licença `Free`; `[AllowLicenseFreeFilter]` inverte em action.

### 6.5 Middleware de erro

`ValidationExceptionMiddleware` traduz exceções de domínio em HTTP:

| Exceção | HTTP | Corpo |
|---|---|---|
| `EntityValidationException` | 400 | `ValidationResult { Errors: [...] }` (JSON) |
| `EntityOperationException` | 400 | `ValidationResult { Errors: [...] }` + header `OperationId` |
| `EntityNotFoundException` | 404 | vazio + header `Reason` |
| Outras | 500 | vazio + header `Reason` |

Log via `TelemetryClient.TrackException`.

**Problema**: inconsistência (às vezes body, às vezes header) e **não é RFC 7807 ProblemDetails**. No novo projeto use `IExceptionHandler` (.NET 8) com ProblemDetails padronizado.

### 6.6 Autenticação / Autorização

- JWT Bearer: `IssuerSigningKey`, `ValidIssuer`, `ValidAudience` vindos de `Configuration["Auth:*"]`. `ClockSkew = 0`.
- Claims customizados: `UserId`, `BusinessUnitId`, `License`, `UserName`, `UserEmail`.
- Policy `"Bearer"` + `HasScopeHandler` (valida `scope` claim).
- `[AllowAnonymous]` usado em downloads públicos e alguns fluxos de onboarding.

⚠️ **Endpoints com `[AllowAnonymous]` em controllers de negócio** (ex.: `CargaController.RecuperarCarga`, `CargaController.EditarCarga`) precisam de revisão de segurança.

### 6.7 SignalR (7 Hubs)

`/cargashub`, `/oferecimentohub`, `/ocorrenciashub`, `/remessashub`, `/certificacaohub`, `/appmotoristahub`, `/alertarecomendacaohub`.

Padrão mínimo — apenas `Groups.AddToGroupAsync`. Publicação feita por `IHubContext<THub>` injetado em App Services. Serviço Azure SignalR gerenciado (connection string `FretefyHub`).

### 6.8 Consumers (MassTransit)

Em `WebApi/Consumers/`:
- `CadastrarCargaConsumer : IConsumer<CadastrarOfertaCarga>` — valida, mapeia, cria carga, publica `OfertaCargaCadastrada` / `OfertaCargaCadastroErro`. Classe de **672 linhas** — urgente quebrar.
- `ReducerConsumer`.

Padrão de configuração: `PrefetchCount = 1`, `MaxConcurrentCalls = 1` (conservador — processamento serial por partição).

### 6.9 Swagger

Swashbuckle **5.5.1**, documentação apenas em `#if DEBUG`. Sem Bearer no UI, sem comentários XML, sem versionamento. **Modernizar no novo projeto** (ver §9.4).

---

## 7. Convenções Transversais

### 7.1 Multi-tenancy

Três níveis de segregação:

1. **Owner** (`IMustHaveOwner`): tenant raiz (embarcadora/transportadora).
2. **UnidadeNegocio** (`IMustHaveBusinessUnit`): subdivisão do Owner.
3. **Holding** (`IMayHaveHolding`): opcional, para grupos corporativos.

Injeção automática via resolvers (`IOwnerResolver`, `IBusinessUnitResolver`, `IUserResolver`) no `SaveChanges` + Global Query Filters no `DbContext`. **Essencial manter** — é o coração do SaaS multi-tenant.

### 7.2 Auditoria

`IMustAudited` → `CreationUserId`, `CreationTime`, `ModificationUserId`, `ModificationTime`. Preenchidos automaticamente no `UnitOfWork.SaveChanges`. Logs críticos de acesso (autenticação, edição de dados sensíveis) em Azure Table Storage (`SecurityLogger`).

### 7.3 Naming

- **Português** para métodos de domínio (`Ativar`, `Incluir`, `Validar`, `Recuperar`).
- **Inglês** para infraestrutura técnica (`Repository`, `Service`, `DbContext`).
- Prefixo `E` em enums (`ETipoMotorista`, `ESituacaoCarga`).
- Sufixo `ValueObject` em VOs, `Dto` em DTOs, `AppService` em Application Services.
- Interfaces com prefixo `I`.

### 7.4 Constants

`Domain/Constants.cs` centraliza IDs "semeados" (GUIDs fixos) para tipos, módulos, produtos, pipes de Kanban. **Evite o crescimento desordenado** — agrupe por bounded context em classes aninhadas (`Constants.Tipos.TiposVeiculo.*`).

---

## 8. Débitos Técnicos e Code Smells Conhecidos

Dívidas identificadas que **NÃO devem ser replicadas** no novo projeto:

### 8.1 Vazamento de infra no Domain

`Domain.csproj` referencia:
- `Microsoft.EntityFrameworkCore 2.2.6` ❌
- `WindowsAzure.Storage 9.3.3` ❌
- `Microsoft.Extensions.DependencyInjection 3.1.7` (aceitável apenas para `IServiceCollection` extensions)
- `Newtonsoft.Json 13.0.1` ❌

### 8.2 DTOs no Domain

Pasta `Domain/DTO/` com 48+ arquivos. DTOs são artefato de Application/API, **não** de Domain. Migrar para `Application/DTO/`.

### 8.3 Pastas de negócio dentro de Infra

`Infra/Application/`, `Infra/Flow/`, `Infra/Freight/` — vazamento inverso (regras de negócio na Infra).

### 8.4 Ausência de Domain Events

Comunicação entre agregados via `IEventBus` em App Services (acoplamento alto). Introduzir Domain Events — §9.3.

### 8.5 God Services

Serviços com 15-17 dependências injetadas (`AcordosAppService`, `VeiculoAppService`). Quebrar por caso de uso.

### 8.6 Anti-patterns de query

- `ToList()` antes de `Where()` (exemplo: `VeiculoRepository.cs:44`).
- Falta sistemática de `AsNoTracking()` em leituras.
- `IQueryable` exposto via `Repository.Query(q => ...)`.

### 8.7 Pacotes desatualizados / vulneráveis

| Pacote | Versão | Status | Ação |
|---|---|---|---|
| `Microsoft.EntityFrameworkCore` | 2.2.6 | 🔴 EOL 12/2022 | → 8.0.x |
| `WindowsAzure.Storage` | 9.3.3 | 🔴 Depreciado (~2018) | → `Azure.Storage.Blobs` 12.x |
| `AutoMapper` | 8.1.0 | 🟠 2019 | → 13.x ou Mapperly |
| `AutoMapper.Collection` | 5.0.0 | 🟠 2018 | → 10.x |
| `Microsoft.ApplicationInsights` | 2.9.1 | 🟠 2018 | → 2.22.x ou OpenTelemetry |
| `Microsoft.AspNet.WebApi.Client` | 5.2.7 | 🔴 EOL 2017 | → `System.Net.Http.Json` |
| `Swashbuckle.AspNetCore` | 5.5.1 | 🟠 | → 6.x ou `Microsoft.AspNetCore.OpenApi` (nativo .NET 9+) |
| `MassTransit` | 7.3.1 | 🟠 | → 8.x (breaking changes) |
| `StackExchange.Redis` | 2.2.4 | 🟡 | → 2.8.x |
| `Newtonsoft.Json` | 13.0.1 | 🟢 ok para compat, mas preferir `System.Text.Json` |

### 8.8 Segurança (CRÍTICO)

- ⚠️ **Credenciais de produção em `appsettings.Development.json`** (Azure SQL password, Service Bus key, Blob Storage key, MongoDB connection, SignalR access key, JWT secret). Rotacionar **imediatamente** e mover para Key Vault / User Secrets / variáveis de ambiente. Adicionar `appsettings.*.json` ao `.gitignore` (mantendo apenas o `appsettings.json` com placeholders).
- ⚠️ **JWT secret de 64 caracteres commitado** em `appsettings.json`.
- ⚠️ API keys (FCM, SendGrid, MapBox, Notion, TalkJS, Twilio) expostas em arquivos versionados.
- ⚠️ `[AllowAnonymous]` em endpoints de `CargaController` — revisar.
- ⚠️ CORS com `AllowAnyMethod()` + `AllowAnyHeader()` — restringir.
- ⚠️ Sem rate limiting em nenhuma camada.
- ⚠️ Sem headers de segurança (CSP, X-Frame-Options, X-Content-Type-Options).

### 8.9 Validação fraca no border

DTOs sem validação — confiam no Domain. Deveria haver FluentValidation na borda da API para falhar rápido sem entrar no Domain.

---

## 9. Guia de Migração para .NET 8

### 9.1 Stack-alvo

| Camada | De | Para |
|---|---|---|
| TFM (Domain/App/Infra) | `netstandard2.1` | `net8.0` |
| TFM (WebApi) | `netcoreapp3.1` | `net8.0` |
| EF Core | 2.2.6 | 8.0.x |
| AutoMapper | 8.1.0 | 13.x ou Mapperly |
| MassTransit | 7.3.1 | 8.x |
| JSON | Newtonsoft | `System.Text.Json` (padrão) |
| Logging | `ILogger` + App Insights 2.9 | `ILogger` + OpenTelemetry + App Insights 2.22 |
| Cache | Redis 2.2.4 | Redis 2.8.x |
| Storage | `WindowsAzure.Storage` 9.3.3 | `Azure.Storage.Blobs` 12.x |
| Swagger | Swashbuckle 5.5 | Swashbuckle 6.x ou `Microsoft.AspNetCore.OpenApi` |

### 9.2 Modernizações seguras (não mudam filosofia)

- **Primary constructors** em App Services e Controllers para reduzir boilerplate:
  ```csharp
  public class MotoristaAppService(
      IUnitOfWork<Motorista> uow,
      IMotoristaService service,
      IEventBus bus) : AppService<Motorista, MotoristaDto>(uow, service, bus), IMotoristaAppService { }
  ```
- **File-scoped namespaces** em todos os arquivos.
- **Nullable reference types** `<Nullable>enable</Nullable>` em todos os `.csproj`.
- **`required`** nas propriedades obrigatórias de DTOs.
- **`record`** para DTOs imutáveis e VOs novos.
- **Init-only** para propriedades de entidades que só são setadas em construtor.
- **Collection expressions** (`[]`, `[..x, y]`) onde legível.

### 9.3 Domain Events (introduzir)

```csharp
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public abstract class AggregateRoot : EntityOwnedAudited
{
    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _events;
    protected void AddDomainEvent(IDomainEvent e) => _events.Add(e);
    public void ClearDomainEvents() => _events.Clear();
}

public sealed record MotoristaAtivado(Guid MotoristaId, Guid OwnerId, DateTime OccurredOn) : IDomainEvent;
```

Dispatch no `SaveChangesAsync` do `DbContext`:

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    var events = ChangeTracker.Entries<AggregateRoot>()
        .SelectMany(e => e.Entity.DomainEvents).ToList();
    ChangeTracker.Entries<AggregateRoot>().ToList()
        .ForEach(e => e.Entity.ClearDomainEvents());

    var result = await base.SaveChangesAsync(ct);
    foreach (var evt in events) await _dispatcher.DispatchAsync(evt, ct);
    return result;
}
```

Cuidado: decidir se o dispatch é *in-process* (MediatR) ou *out-of-process* (Service Bus). O atual já publica via MassTransit; Domain Events podem continuar sendo convertidos em Integration Events no dispatcher.

### 9.4 WebApi moderna

- Minimal APIs **são opcionais** — o padrão atual (Controllers) é legítimo em .NET 8 e mais familiar para o time. **Recomendação: manter Controllers** por fidelidade, mas usar endpoint filters e `TypedResults`.
- **ProblemDetails RFC 7807** via `IExceptionHandler`:
  ```csharp
  builder.Services.AddProblemDetails();
  builder.Services.AddExceptionHandler<DomainExceptionHandler>();
  ```
- **API Versioning** com `Asp.Versioning.Mvc` (header ou query).
- **Swagger 6.x** com Bearer configurado, XML comments, exemplos.
- **Rate limiting** nativo (.NET 7+):
  ```csharp
  builder.Services.AddRateLimiter(o => o
      .AddFixedWindowLimiter("api", opts => { opts.PermitLimit = 100; opts.Window = TimeSpan.FromMinutes(1); }));
  ```
- **Health checks** `MapHealthChecks("/health")` com probes específicas (SQL, Redis, Service Bus).

### 9.5 DomainException base

```csharp
public abstract class DomainException : Exception
{
    public string Code { get; }
    protected DomainException(string code, string message, Exception? inner = null)
        : base(message, inner) { Code = code; }
}

public sealed class EntityValidationException : DomainException
{
    public ValidationResult Result { get; }
    public EntityValidationException(ValidationResult r)
        : base("ENTITY_VALIDATION", r.ToString()) { Result = r; }
}
```

O middleware de erro ganha um só catch para `DomainException` e usa `Code` como `type` do ProblemDetails.

### 9.6 Performance

- **`AsNoTracking()`** em todas as queries de leitura (adicionar como extension method obrigatório).
- **`AsSplitQuery()`** em `Include` múltiplos para evitar explosão cartesiana.
- **Compiled queries** para hot paths.
- **Paginação obrigatória** em listagens (`Skip`/`Take`) — nunca `.ToList()` sem filtro.
- **Projeções em DTO** (`.Select(x => new XDto { ... })`) em vez de materializar a entidade.
- **`IAsyncEnumerable<T>`** para streams grandes.
- **Mapperly** (source generator) para mapeamentos críticos — elimina reflection.
- **Prefetch** do Service Bus revisado — `MaxConcurrentCalls = 1` é conservador demais para consumers idempotentes.

### 9.7 DTOs modernos

```csharp
public sealed record MotoristaCreateDto
{
    public required string Nome { get; init; }
    public required string Documento { get; init; }
    public required string Cnh { get; init; }
    public string? Email { get; init; }
}

// FluentValidation na borda
public sealed class MotoristaCreateDtoValidator : AbstractValidator<MotoristaCreateDto>
{
    public MotoristaCreateDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Documento).NotEmpty().Must(CpfCnpjValidator.IsValid);
        RuleFor(x => x.Cnh).NotEmpty().MaximumLength(15);
    }
}
```

### 9.8 Segurança — ação obrigatória

1. **Rotacionar todas as credenciais comprometidas** (SQL, Service Bus, Blob, MongoDB, SignalR, JWT, API keys).
2. **Azure Key Vault** para produção/homolog; **User Secrets** (`dotnet user-secrets`) para desenvolvimento.
3. `.gitignore` com `appsettings.Development.json`, `appsettings.Local.json`.
4. **CORS restritivo** com origens explícitas por ambiente, métodos/headers listados.
5. **HSTS** + **HTTPS redirection**.
6. **Security headers** via middleware: `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy: strict-origin-when-cross-origin`, CSP apropriado.
7. **Rate limiting** especialmente em `auth/*` e endpoints com `[AllowAnonymous]`.
8. **Revisar `[AllowAnonymous]`** endpoint por endpoint — cada um exige justificativa escrita.
9. JWT: manter `ClockSkew = 0`, adicionar **refresh tokens** com rotação, considerar `ValidateAudience` + `ValidateIssuer` sempre ativo.
10. Auditoria: substituir Azure Table Storage por **solução append-only** (p.ex. Event Hubs + Blob) ou manter Table mas com retention policy explícita.

### 9.9 Observabilidade

- **OpenTelemetry** (traces + metrics + logs) → Application Insights como exporter.
- **Correlation ID** middleware (propaga `traceparent` W3C).
- Logs estruturados: `ILogger` com templates, evitar interpolação de string.

### 9.10 Infra de build/CI

- Incluir `dotnet list package --vulnerable --include-transitive` no CI.
- **CodeQL** / **SonarCloud** no pipeline.
- `.editorconfig` + analyzers (`Microsoft.CodeAnalysis.NetAnalyzers`, `Meziantou.Analyzer`).
- **Directory.Packages.props** (Central Package Management) para unificar versões.

---

## 10. Checklist para Novo Projeto

### Infra de repositório

- [ ] Directory.Packages.props (CPM) em todos os csproj.
- [ ] `<Nullable>enable</Nullable>` e `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` globais.
- [ ] `.gitignore` cobrindo `appsettings.*.json` (exceto `.json` com placeholders).
- [ ] `.editorconfig` + analyzers.
- [ ] Pipeline CI com build + testes + vulnerabilidade de pacotes.

### Domain (novo)

- [ ] Projeto `Domain.csproj` **sem referências** a EF, Azure, DI, Newtonsoft.
- [ ] Hierarquia `Entity → EntityAudited → AggregateRoot` com Domain Events.
- [ ] Interfaces `IMustHaveOwner`, `IMustHaveBusinessUnit`, `IMustAudited`, `IAggregateRoot`.
- [ ] `DomainException` base + `EntityValidationException` / `EntityNotFoundException` derivadas.
- [ ] VOs reutilizados: `EmailValueObject`, `CPFCNPJValueObject`, `TelefoneValueObject`, `CidadeValueObject`, `PlacaValueObject`, `RNTRCValueObject`, `StorageValueObject`, `CNHValueObject`.
- [ ] Enums com prefixo `E` e `[Description]`.
- [ ] Specifications em `Validations/Specifications/`.
- [ ] `IRepository<T>` + interfaces específicas por agregado.
- [ ] Domain Services em `Services/` — sem chamar Application.

### Application (novo)

- [ ] `AppService<T, TDto>` base preservado.
- [ ] DTOs como `record` com `required`.
- [ ] FluentValidation para DTOs.
- [ ] AutoMapper 13 **ou** Mapperly; profiles por bounded context.
- [ ] `IUnitOfWork<T>` com Begin/Confirm + transação explícita.
- [ ] Separação `Web/` vs `Mobile/` quando houver múltiplos canais.
- [ ] Sem lógica de negócio "else" (validação que comparam estado de outros agregados → Domain Service).

### Infra (novo)

- [ ] EF Core 8 + `Azure.Storage.Blobs 12.x` + Redis 2.8 + Mongo.Driver latest.
- [ ] `FreteFyDbContext` com resolvers injetados + global filters multi-tenant.
- [ ] `UnitOfWork` injetando `OwnerId`/`UnidadeNegocioId`/auditoria em `SaveChanges`.
- [ ] Repositórios base + específicos; **todas** as leituras com `AsNoTracking`.
- [ ] MongoDB para read models (se aplicável).
- [ ] Interfaces de integrações no Domain/Application; implementações na Infra.
- [ ] Configuração via `IOptions<T>`.
- [ ] Sem `Infra/Application/`, `Infra/Flow/` etc. — mover para Application/Domain.

### WebApi (novo)

- [ ] Program.cs minimal host (`WebApplication.CreateBuilder`).
- [ ] Pipeline: HTTPS → HSTS → SecurityHeaders → Routing → CORS → Auth → RateLimit → Authorization → ExceptionHandler (ProblemDetails) → Endpoints.
- [ ] `IExceptionHandler` + `AddProblemDetails()`.
- [ ] JWT Bearer + refresh tokens + rotação.
- [ ] API Versioning via header/query.
- [ ] Swagger 6 com Bearer + XML comments + exemplos.
- [ ] Rate limiting por endpoint crítico.
- [ ] Health checks SQL/Redis/ServiceBus.
- [ ] Controllers thin (validação delegada a FluentValidation).
- [ ] Consumers < 200 linhas; orquestração em App Service.

### Segurança (obrigatório dia 1)

- [ ] Todas as credenciais em **Azure Key Vault** (ou User Secrets para dev).
- [ ] `.gitignore` valida que nenhum secret vai para o repo.
- [ ] CORS restritivo por origem.
- [ ] Security headers aplicados.
- [ ] Rate limit em `auth/*`.
- [ ] Revisão formal de cada `[AllowAnonymous]`.
- [ ] Auditoria append-only com retention definida.

### Observabilidade

- [ ] OpenTelemetry (traces + metrics + logs).
- [ ] Correlation ID.
- [ ] Logs estruturados com `ILogger`.

### Testes

- [ ] Unit tests no Domain (Value Objects, Entities, Domain Services — 80%+ cobertura).
- [ ] Integration tests para repositórios com `Testcontainers` (SQL + Mongo).
- [ ] Contract tests para consumers de Service Bus.

---

## Apêndice A — Referências do código atual

| Tópico | Arquivo |
|---|---|
| Solução | `PG.FreteFy.sln` |
| DbContext | `PG.FreteFy.Infra/EntityFramework/FreteFyDbContext.cs` |
| UnitOfWork | `PG.FreteFy.Infra/EntityFramework/UnitOfWork/UnitOfWork.cs` |
| Mapping exemplar | `PG.FreteFy.Infra/EntityFramework/Mappings/AccountMap.cs` |
| App Service base | `PG.FreteFy.Application/Services/EntityAppService.cs`, `DtoAppService.cs` |
| Mapper profile | `PG.FreteFy.Application/ApplicationMapperProfile.cs` |
| Domain container | `PG.FreteFy.Domain/DomainContainer.cs` |
| Entidade auditada | `PG.FreteFy.Domain/Entities/EntityOwnedAudited.cs` |
| VO exemplar | `PG.FreteFy.Domain/ValueObjects/TelefoneValueObject.cs` |
| Entity validation | `PG.FreteFy.Domain/Validations/MotoristaEntityValidation.cs` |
| Exception middleware | `PG.FreteFy.WebApi/Infra/ValidationHandlingMiddleware.cs` |
| Auth filter | `PG.FreteFy.WebApi/Infra/BusinessUnitAuthorizationFilter.cs` |
| Startup | `PG.FreteFy.WebApi/Startup.cs` |
| Constants | `PG.FreteFy.Domain/Constants.cs` |

---

_Última atualização: 2026-04-20._
