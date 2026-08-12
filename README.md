<!-- Improved compatibility of back to top link -->
<a id="readme-top"></a>

# Todo.Classic.API

A classic layered ASP.NET Core Web API for managing to-do items, targeting **.NET 10**. The solution demonstrates a clean separation of concerns across dedicated projects for API, business logic, data access, domain/DTO models, and shared helpers.

## Table of Contents

- [Solution Layout](#solution-layout)
- [Features](#features)
- [Architecture](#architecture)
- [Built With](#built-with)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Build & Run](#build--run)
  - [Database](#database)

## Solution Layout

The solution (Todo.Classic.API.slnx) is composed of five projects:

| Project | Purpose |
| --- | --- |
| `Todo.Classic.API` | ASP.NET Core Web API host. Exposes REST endpoints via `TodoController`, wires up dependency injection, OpenAPI, HTTPS redirection, and Serilog-based structured logging in `Program.cs`. |
| `Todo.Classic.BusinessLogic` | Business rules and orchestration. Contains `ITodoService`/`TodoService` for todo operations and factories (e.g., `TodoItemFactory`) that validate input and construct domain entities. |
| `Todo.Classic.DataAccess` | Entity Framework Core data layer with `TodoDbContext` mapping domain entities to the database. |
| `Todo.Classic.Model` | Domain entities (`TodoItem`) and DTOs (`CreateTodoRequest`, `TodoItemDto`) shared across layers. |
| `Todo.Classic` | Shared helpers and cross-cutting types, including `BusinessLogicException` used to signal validation/rule failures back to the API layer. |

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

## Architecture

- **Layered architecture** – Controllers stay thin; validation and rules live in the business logic layer, and persistence is isolated in the data access project.
- **DTO-based contracts** – Requests and responses use records (`CreateTodoRequest`, `TodoItemDto`) to keep API and domain models decoupled.
- **Consistent error handling** – Controllers translate `BusinessLogicException` to `400 Bad Request`, missing resources to `404 Not Found`, and unexpected errors to `500 Internal Server Error`, with structured logging around each request.
- **Structured logging** – Serilog is configured at application startup and used throughout services and controllers with contextual properties.
- **OpenAPI** – Registered via `AddOpenApi()` for API discovery and tooling.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built With

| Logo | Technology | Purpose |
| :---: | --- | --- |
| ![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) | **.NET 10 / ASP.NET Core** | ASP.NET Controller APIs host and runtime |
| ![EF Core](https://img.shields.io/badge/EF_Core_10-512BD4?style=for-the-badge&logo=nuget&logoColor=white) | **Entity Framework Core 10** | Data access (in-memory for dev; SQL Server package included) |
| ![Serilog](https://img.shields.io/badge/Serilog-4B8BBE?style=for-the-badge&logo=serilog&logoColor=white) | **Serilog** | Structured logging |
| ![OpenAPI](https://img.shields.io/badge/OpenAPI-6BA539?style=for-the-badge&logo=openapiinitiative&logoColor=white) | **OpenAPI** | API description via `AddOpenApi()` |
| [![Scalar](https://img.shields.io/badge/Scalar-1B1F23?style=for-the-badge&logo=scalar&logoColor=white)](https://scalar.com/) | **Scalar** | Interactive API document UI |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) (Community edition or higher) with the **ASP.NET and web development** workload.

### Build & Run

1. Clone the repository:

   ```powershell
   git clone https://github.com/andygroat/todo-classic-api.git
   cd todo-classic-api
   ```

2. Open `Todo.Classic.API/Todo.Classic.API.slnx` in Visual Studio 2026.
3. Restore NuGet packages (Visual Studio does this automatically on load, or run `dotnet restore`).
4. Configure the database connection string in `Todo.Classic.API/Todo.Classic.API/appsettings.json` (used by `TodoDbContext`).
5. Set `Todo.Classic.API` as the startup project and run. The OpenAPI document will be available for exploring the endpoints.

The API starts on the URLs listed in `Todo.Classic.Api/Properties/launchSettings.json`. OpenAPI and Scalar UI are enabled for exploring the endpoints.

### Database

By default the API registers `TodoDbContext` with EF Core's **in-memory** provider (`TodoDb`) for zero-setup local development. To switch to a real provider (e.g., SQL Server), replace the registration in `Infrastructure/Extensions/WebApplicationBuilderExtensions.AddDatabaseContext` and supply a connection string via `appsettings.json` -> `ConnectionStrings`.

<p align="right">(<a href="#readme-top">back to top</a>)</p>
