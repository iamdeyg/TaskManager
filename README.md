# MyNewApp — Todo Manager API

A simple in-memory Todo/Task manager REST API built with ASP.NET Core Minimal APIs on .NET 10,
created as a hands-on learning project for backend web development with .NET.

## Features

- Add, retrieve, and delete todos
- Validates that todos cannot have a due date in the past
- Rejects todos that are already marked as completed on creation
- URL redirect middleware (`/tasks/*` → `/todos/*`)
- Request logging via custom middleware

## Endpoints

| Method | Route         | Description              |
|--------|---------------|--------------------------|
| GET    | /todos        | Get all todos            |
| GET    | /todos/{id}   | Get a todo by ID         |
| POST   | /todos        | Create a new todo        |
| DELETE | /todos/{id}   | Delete a todo by ID      |

### Todo Object

```json
{
  "id": 1,
  "title": "Buy groceries",
  "isCompleted": false,
  "dueDate": "2026-06-15T10:00:00Z"
}
```

### Validation Rules (POST /todos)

- `dueDate` must not be in the past
- `isCompleted` must be `false` on creation

### Validation Error Response (400)

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "DueDate": ["Due date cannot be in the past."]
  }
}
```

## Tech Stack

- .NET 10
- ASP.NET Core Minimal APIs
- In-memory storage (no database)

## Running the Project

```bash
dotnet run
```

## What I Learned

- **APIs, Web APIs, and ASP.NET Core** — understanding what APIs are and how ASP.NET Core fits in
- **Hosting model and application customization** — configuring the app via `WebApplication` and `WebApplicationBuilder`
- **Endpoint routing and minimal APIs** — defining routes with `MapGet`, `MapPost`, `MapDelete`
- **Middlewares** — built-in (`UseRewriter`) and custom request logging middleware
- **Endpoint Filters** — request validation with `AddEndpointFilter` and RFC 9110 problem details responses
- **Dependency Injection** — registering and consuming services (`ITaskservice` / `InMemoryTaskService`)
