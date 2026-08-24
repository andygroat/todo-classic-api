# copilot-instructions.md

## Overview
[Todo.Classic.API] - Classic layered Web API

## Tech Stack

- **.NET 10** / **ASP.NET Core** (controller-based Web API).
- **Entity Framework Core 10** (InMemory provider by default; SQL Server package available).
- **Serilog** / **Serilog.AspNetCore** for structured logging.
- **OpenAPI** via `AddOpenApi()` and **Scalar.AspNetCore** for the interactive UI.
- **NUnit 4**, **NSubstitute**, **coverlet.collector** for tests.
- `Nullable` and `ImplicitUsings` are **enabled** in every project.

## Project Structure

`Todo.Classic.API` is a classic layered ASP.NET Core Web API for managing to-do items, targeting **.NET 10**.

| Project | Purpose |
| --- | --- |
| `Todo.Classic.API` | ASP.NET Core Web API host. Controllers, DI wiring, OpenAPI, HTTPS redirection, Serilog logging in `Program.cs`. |
| `Todo.Classic.BusinessLogic` | Business rules and orchestration (`ITodoService`/`TodoService`, factories like `TodoItemFactory`). |
| `Todo.Classic.DataAccess` | EF Core data layer with `TodoDbContext`. |
| `Todo.Classic.Model` | Domain entities (`TodoItem`) and DTOs (`CreateTodoRequest`, `TodoItemDto`). |
| `Todo.Classic` | Shared/cross-cutting types, including `BusinessLogicException`. |
| `Todo.Classic.API.Tests` | NUnit tests for the API layer. |
| `Todo.Classic.BusinessLogic.Tests` | NUnit tests for the business logic layer. |

## Code Conventions

- Target framework is `net10.0` — use modern C#/.NET 10 language and API features.
- Respect nullable reference types; annotate nullability explicitly and avoid `!` unless justified.
- Prefer `record` types for DTOs (matches existing `CreateTodoRequest`, `TodoItemDto`).
- Keep controllers thin: no validation or business rules in controllers.
- Put validation and orchestration in the `BusinessLogic` layer; construct entities via factories (e.g., `TodoItemFactory`).
- Keep EF Core `DbContext` usage confined to `Todo.Classic.DataAccess`.
- Use dependency injection; register services via extension methods under `Todo.Classic.API/Infrastructure/Extensions/` (e.g., `WebApplicationBuilderExtensions.AddDatabaseContext`).
- Rely on `ImplicitUsings`; avoid adding redundant `using` directives.
- Follow existing folder and namespace layout per project.
- File-scoped namespaces.
- Primary constructors for dependency injection.
- Sealed classes for implementations.
- Internal by default, public only for contracts.
- Naming: PascalCase for types/members, camelCase for locals/parameters, `_camelCase` for private fields.
- Prefer `async`/`await` end-to-end; accept and propagate `CancellationToken`.

## When Adding Code

1. New endpoints should live on `TodoController` (or a new controller with the same conventions) and accept/return DTO records from `Todo.Classic.Model`.
2. Keep NuGet package versions aligned across projects.

## Error Handling

- Signal validation / business rule failures by throwing `BusinessLogicException` from the business logic layer.
- Controllers translate exceptions consistently:
  - `BusinessLogicException` → `400 Bad Request`
  - Missing resource → `404 Not Found`
  - Unexpected exception → `500 Internal Server Error`
- Log around each request with structured Serilog properties; do not use `Console.WriteLine`.

## Testing

- Test framework: **NUnit 4** with `NSubstitute` for mocking. `NUnit.Framework` is a global `Using` in both test projects.
- Add API tests to `Todo.Classic.API.Tests`, business logic tests to `Todo.Classic.BusinessLogic.Tests`.
- `Todo.Classic.BusinessLogic` exposes internals to `Todo.Classic.BusinessLogic.Tests` via `InternalsVisibleTo` — internal types can be tested directly.
- Prefer AAA (Arrange / Act / Assert) structure and NUnit constraint-model assertions (`Assert.That(...)`).
- Use EF Core InMemory provider for handler/integration tests inside modules.
- Code coverage is collected via `coverlet.collector` using `Todo.Classic.API/.runsettings`; keep new test assemblies compatible with those settings.
- Both test projects run on the Microsoft Testing Platform (`IsTestingPlatformApplication=true`, `OutputType=Exe`); do not change these.

## Logging

- Use Serilog via injected `ILogger<T>`.
- Include contextual properties (e.g., `todoId`, `search`) using message templates, not string interpolation.

## Do

- ✅ Do keep changes within the appropriate layer and honor existing project references.
- ✅ Do add new DI registrations via extension methods under `Infrastructure/Extensions/`.
- ✅ Do target `.NET 10` idioms (primary constructors, collection expressions, `required` members where sensible).

## Don't

- ❌ Don't reference `Todo.Classic.DataAccess` from `Todo.Classic.API` controllers directly for business operations — go through `BusinessLogic`.
- ❌ Don't introduce new logging frameworks or swap out Serilog.
- ❌ Don't change target frameworks or downgrade package versions.
