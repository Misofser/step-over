# StepOver App

A small full-stack application for managing personal goals.

Built with **ASP.NET Core** on the backend and **React + TypeScript** on the frontend.

---

## Preview

![StepOver App screenshot](./screenshots/app.png)

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

## Backend Setup

```bash
cd step-over-backend/GoalApi
dotnet restore
dotnet ef database update
dotnet run
```

---

## Frontend Setup

```bash
cd step-over-frontend/goal-frontend
npm install
npm run dev
```
