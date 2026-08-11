<!-- Improved compatibility of back to top link -->
<a id="readme-top"></a>

# Todo.Classic.API

A classic layered ASP.NET Core Web API for managing to-do items, targeting **.NET 10**. The solution demonstrates a clean separation of concerns across dedicated projects for API, business logic, data access, domain/DTO models, and shared helpers.

## Table of Contents

- [Solution Structure](#solution-structure)
- [Features](#features)
- [Key Design Choices](#key-design-choices)
- [Built With](#built-with)
- [Getting Started](#getting-started)

## Solution Structure

- **`Todo.Classic.API`** – ASP.NET Core Web API host. Exposes REST endpoints via `TodoController`, wires up dependency injection, OpenAPI, HTTPS redirection, and Serilog-based structured logging in `Program.cs`.
- **`Todo.Classic.BusinessLogic`** – Business rules and orchestration. Contains `ITodoService`/`TodoService` for todo operations and factories (e.g., `TodoItemFactory`) that validate input and construct domain entities.
- **`Todo.Classic.DataAccess`** – Entity Framework Core data layer with `TodoDbContext` mapping domain entities to the database.
- **`Todo.Classic.Model`** – Domain entities (`TodoItem`) and DTOs (`CreateTodoRequest`, `TodoItemDto`) shared across layers.
- **`Todo.Classic`** – Shared helpers and cross-cutting types, including `BusinessLogicException` used to signal validation/rule failures back to the API layer.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Features

The API currently supports the following todo operations:

| Method | Route                        | Description                                          |
| ------ | ---------------------------- | ---------------------------------------------------- |
| POST   | `/api/todo`                  | Create a new todo item.                              |
| GET    | `/api/todo?search=...`       | List todo items, optionally filtered by description. |
| GET    | `/api/todo/{id}`             | Retrieve a single todo item by id.                   |
| POST   | `/api/todo/{id}/complete`    | Mark a todo item as completed.                       |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Key Design Choices

- **Layered architecture** – Controllers stay thin; validation and rules live in the business logic layer, and persistence is isolated in the data access project.
- **DTO-based contracts** – Requests and responses use records (`CreateTodoRequest`, `TodoItemDto`) to keep API and domain models decoupled.
- **Consistent error handling** – Controllers translate `BusinessLogicException` to `400 Bad Request`, missing resources to `404 Not Found`, and unexpected errors to `500 Internal Server Error`, with structured logging around each request.
- **Structured logging** – Serilog is configured at application startup and used throughout services and controllers with contextual properties.
- **OpenAPI** – Registered via `AddOpenApi()` for API discovery and tooling.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built With

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=for-the-badge&logo=nuget&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![Serilog](https://img.shields.io/badge/Serilog-000000?style=for-the-badge&logo=serilog&logoColor=white)](https://serilog.net/)
[![Scalar](https://img.shields.io/badge/Scalar-1E1E2E?style=for-the-badge&logo=scalar&logoColor=white)](https://scalar.com/)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting Started

### Prerequisites

- [Visual Studio 2026](https://visualstudio.microsoft.com/) (Community edition or higher) with the **ASP.NET and web development** workload.
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/)
- A supported database engine for Entity Framework Core (e.g., SQL Server, SQL Server LocalDB, or another provider configured in `TodoDbContext`).

### Installation

1. Clone the repository:

   ```powershell
   git clone https://github.com/andygroat/todo-classic-api.git
   cd todo-classic-api
   ```

2. Open `Todo.Classic.API/Todo.Classic.API.slnx` in Visual Studio 2026.
3. Restore NuGet packages (Visual Studio does this automatically on load, or run `dotnet restore`).
4. Configure the database connection string in `Todo.Classic.API/Todo.Classic.API/appsettings.json` (used by `TodoDbContext`).
5. Apply any pending EF Core migrations, if applicable:

   ```powershell
   dotnet ef database update --project Todo.Classic.API/Todo.Classic.DataAccess --startup-project Todo.Classic.API/Todo.Classic.API
   ```

6. Set `Todo.Classic.API` as the startup project and run. The OpenAPI document will be available for exploring the endpoints.

<p align="right">(<a href="#readme-top">back to top</a>)</p>
