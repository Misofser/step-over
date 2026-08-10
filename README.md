# StepOver App

A full-stack goal planning application focused on personal productivity, designed with future support for collaborative workspaces. Users can organize goals, habits, and tasks, and track their progress through analytics and structured planning.

Built with **ASP.NET Core** on the backend and **React + TypeScript** on the frontend.

---

## Preview

### Today Dashboard
![Today Dashboard](./screenshots/app_1.png)

### Goals
![Goals](./screenshots/app_2.png)

### Goal Details
![Goal Details](./screenshots/app_3.png)

## Tech Stack

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger (OpenAPI)

### Frontend
- React
- TypeScript
- Vite

---

## Development Setup Only

### Backend Setup

1. Copy `appsettings.Development.json.example` to `appsettings.Development.json` and fill in your own admin credentials and JWT secret.  
JWT secret key should be sufficiently long. It is recommended to use at least 32 random characters (256 bits).
> ⚠️ Do **not** use these values in production.

Run backend

```bash
cd step-over-backend/GoalApi
dotnet restore
dotnet ef database update
dotnet run
```

---

### Frontend Setup

2. Copy `env.example` to `env.development`
> ⚠️ Do **not** use these values in production.

Run frontend

```bash
cd step-over-frontend
npm install
npm run dev
```

---

Log in with the dev admin using the credentials you provide in `appsettings.Development.json`.
