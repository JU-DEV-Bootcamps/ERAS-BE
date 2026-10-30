# AGENTS.md — ERAS Backend (`ERAS-BE/`)

Backend rules of ERAS. The shared rules (core principles §1.1, conventions §5, cross-cutting contracts §4.3, workflow §8, limits §9) live in the root `AGENTS.md` of the ERAS parent repository and apply in full here too. Section numbers in this file are the same as in the root file, so "`AGENTS.md` §6.1" means this file. A rule lives in exactly one file (ADR-0005 in `openspec/adr/` of the parent repository); Frontend rules are in the `AGENTS.md` of the frontend repository (`ERAS-FE`).

Commands below are written from the parent repository root; when you are already inside `ERAS-BE/`, skip the `cd ERAS-BE` line.

---

## 3. Commands

### 3.2 Backend (`ERAS-BE/`)

```bash
cd ERAS-BE
dotnet restore Eras.sln
dotnet build Eras.sln -warnaserror                                  # build gate
dotnet format Eras.sln --verify-no-changes                          # format gate
dotnet test Eras.sln                                                # all tests
dotnet test Eras.sln --filter "Change=<change-id>"                  # every AC test of a change
dotnet test Eras.sln --filter "AC=<change-id>/AC-1"                 # one AC
dotnet test Eras.sln --filter "FullyQualifiedName~CreateRemission"  # by name
dotnet test Eras.sln --collect:"XPlat Code Coverage" --results-directory ./TestResults --settings coverlet.runsettings
                                                                    # coverage (CI gate ≥ 25% lines)
dotnet ef migrations add <Name> -p src/Eras.Infrastructure -s src/Eras.Api -o Persistence/PostgreSQL/Migrations
```

---

## 4. Architecture

### 4.1 Backend — .NET 8 Onion Architecture (`ERAS-BE/`)

```
            ┌───────────────────────── Eras.Api ─────────────────────────┐
            │   ┌──────────────── Eras.Infrastructure ────────────────┐  │
            │   │   ┌────────────── Eras.Application ─────────────┐   │  │
            │   │   │   ┌─────────── Eras.Domain ────────────┐    │   │  │
            │   │   │   │  Entities, enums, validators,      │    │   │  │
            │   │   │   │  status transitions                │    │   │  │
            │   │   │   └────────────────────────────────────┘    │   │  │
            │   │   │  Features (CQRS), Contracts, DTOs, Mappers   │   │  │
            │   │   └──────────────────────────────────────────────┘   │  │
            │   │  EF Core/PostgreSQL, repositories, Keycloak, jobs    │  │
            │   └──────────────────────────────────────────────────────┘  │
            │  Controllers, middleware, filters, Program.cs              │
            └────────────────────────────────────────────────────────────┘
            Eras.Error: cross-cutting exception types (referenced by Application and Api)
```

| Layer | Project | Owns | May reference |
|---|---|---|---|
| Domain | `src/Eras.Domain` | Entities (`Entities/`, `Entities/AssessmentManagement/`, `UserManagement/`, `FeatureFlagManagement/`), enums, FluentValidation validators, status transitions (`StatusManagement/`) | nothing |
| Application | `src/Eras.Application` | Use cases (`Features/<Feature>/`), repository and service **interfaces** (`Contracts/`), DTOs (`DTOs/<Area>/`), mappers (`Mappers/`), application validation (`Validation/`), events (`Events/`) | Domain, Eras.Error |
| Infrastructure | `src/Eras.Infrastructure` | `AppDbContext`, EF configurations, persistence entities and joins, repositories, migrations, SQL views, Keycloak/Cosmic Latte clients, file storage, background jobs, authorization policies (`Authorization/ErasPolicies.cs`) | Application, Domain |
| Presentation | `src/Eras.Api` | Controllers, middleware, filters, Swagger, composition root (`Program.cs`) | Application, Infrastructure (only for DI and policies), Eras.Error |
| Errors | `Eras.Error` (repo root) | `ErasException`, `BussinessException`, `NotFoundException`, `CriticalException`, `DatabaseCustomException` | nothing |

---

## 6. Backend

### 6.1 Golden path for a new backend use case

The reference implementation is `Features/RemissionManagement/` together with `Controllers/AssessmentManagement/AssessmentsController.cs`.

1. **Domain.** Add or adjust the entity in `Eras.Domain/Entities/<Area>/`. Put invariants in an `AbstractValidator<T>` under `Validators/` and state changes in `StatusManagement/`.
2. **Contract.** Declare the repository interface in `Eras.Application/Contracts/Persistence/<Area>/I<Name>Repository.cs`.
3. **Request.** Add a `sealed record` to `Features/<Feature>/Commands.cs` or `Queries.cs` that implements `IRequest<TResponse>`.
4. **Handler.** One class per request in `Features/<Feature>/Handlers/<Request>Handler.cs`.
   - Inject `IMapper<TSrc,TDst>`, `IValidator<T>` and the repositories.
   - Return DTOs.
   - Report failures by **throwing** `Eras.Error` exceptions (`NotFoundException`, `BussinessException(msg, statusCode)`).
5. **DTO and mapper.** Put the DTO in `DTOs/<Area>/<Name>Dto.cs` and the mapper in `Mappers/<Area>/` implementing `IMapper<,>`. Register mappers in `ApplicationServiceRegistration`.
6. **Persistence.** Write the repository in `Infrastructure/Persistence/PostgreSQL/Repositories/<Area>/` and the EF configuration in `Configurations/`. Register it in `PersistenceServiceRegistration.cs`. Schema changes require a new migration (§3.2).
7. **Controller.** Keep it thin. Route `api/v1/<kebab-case-resource>`. Use `[Authorize(Policy = ErasPolicies.…)]` on the class or the action. Call `_mediator.Send(request, cancellationToken)` and return `Ok`, `CreatedAtAction` or `NoContent`. Put no business logic here.

### 6.2 Backend rules

- `CancellationToken` flows from the controller action to EF calls. Async methods end in `Async`.
- Use file-scoped namespaces, camelCase parameters and `_camelCase` private fields, Allman braces and 4-space indent. Follow `ERAS-BE/.editorconfig`.
- Inject `ILogger<T>` with the **correct** `T`. Never log tokens, API keys or personal data.
- Authorization uses the role policies in `ErasPolicies` (`RequireErasAdmin`, `RequireErasAdminOrOfficer`, `RequireAnyErasRole`). Ownership and scope rules (for example "a Professional only sees interventions assigned to them") belong in the handler and must be spelled out as ACs.
- Never edit a migration that has been applied. Never hand-edit `AppDbContextModelSnapshot.cs`. SQL views go in `Views/Ups` and `Views/Drops`.
- Secrets come from environment variables or user-secrets, never from committed `appsettings*.json`.
- **Do not copy these legacy patterns into new code:**
  - `BaseResponse` / `CreateCommandResponse<T>` / `Success=false` return values.
  - Static `ToDomain()` / `ToDto()` mappers.
  - DataAnnotations-only validation.
  - Folder-per-use-case `Command/<UseCase>/` layout.
  - Controllers calling application services directly instead of going through MediatR.
  - PascalCase parameters.
  - `try/catch` blocks in controllers.

### 6.3 Backend tests — xUnit (`ERAS-BE/test/`)

| Code under test | Test project |
|---|---|
| Domain validators, transitions | `test/Eras.Domain.Tests/` |
| Handlers, mappers, application services | `test/Eras.Application.Tests/Features/<Feature>/…` |
| Repositories, external clients, jobs | `test/Eras.Infrastructure.Tests/…` |
| Controllers, policies, middleware, filters | `test/Eras.Api.Tests/…` |

Write the test at the layer where the AC's behavior is decided. A business rule is tested in a handler or domain test. A status code or authorization rule is tested in an Api test. Stack: xUnit, Moq, FluentAssertions, EF InMemory/MockQueryable.

```csharp
public class CreateConfigurationCommandHandlerTests
{
    [Fact]
    [Trait("Change", "608-reject-duplicate-configuration-name")]
    [Trait("AC", "608-reject-duplicate-configuration-name/AC-1")]
    public async Task HandleAsync_DuplicateActiveName_Throws409AndDoesNotPersistAsync()
    {
        // Arrange (GIVEN) … Act (WHEN) … Assert (THEN / AND)
    }
}
```

The method name is `<Unit>_<Condition>_<Expected>`. Use Arrange/Act/Assert, mirroring GIVEN/WHEN/THEN.
