# TruckLogix — Truck Logistics Management System

A TQL-style freight brokerage management application built with React.js, C# .NET 8, and SQLite.

## Tech Stack

- **Frontend**: React.js 18 with React Router v6
- **Backend**: ASP.NET Core 8 Web API with Entity Framework Core
- **Database**: SQLite (file-based, created automatically on first run)

## Features

- **Dashboard** — Live stats (total loads, in-transit, revenue)
- **Load Management** — Create, view, assign, and track freight loads
- **Carrier Management** — CRUD carrier profiles with MC/DOT numbers
- **Driver Management** — Driver roster with availability tracking
- **Load Tracking** — Add tracking events (PickedUp, InTransit, Delivered, etc.)
- **Search & Filter** — Search loads by city, commodity, carrier; filter by status

## Project Structure

```
truck-logistics/
├── backend/
│   ├── TruckLogistics.API/     # ASP.NET Core Web API
│   │   ├── Controllers/        # Loads, Carriers, Drivers endpoints
│   │   ├── Models/             # EF Core entities
│   │   ├── Data/               # DbContext
│   │   └── DTOs/               # Request/response shapes
│   └── TruckLogistics.API.Tests/  # xUnit integration tests (in-memory SQLite)
├── frontend/
│   └── src/
│       ├── pages/              # Dashboard, Loads, Carriers, Drivers
│       └── services/           # Axios API client
└── scripts/
    └── seed-sample-data.ps1    # Seeds sample data through the API
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer (a newer SDK can build the `net8.0` target; the .NET 8 runtime is required to run it)
- [Node.js 18+](https://nodejs.org/)

No database server is needed — the API uses a local SQLite file.

### 1. Backend

```bash
cd backend/TruckLogistics.API
dotnet run
# API runs at http://localhost:5050
# Swagger UI: http://localhost:5050/swagger
```

On startup the API creates `trucklogistics.db` if needed and applies any pending EF Core migrations. The database file is git-ignored; delete it to start fresh.

> **Upgrading from an older checkout?** Databases created before migrations were introduced can't be upgraded. The API will stop with a message telling you to delete `trucklogistics.db` (plus its `-shm`/`-wal` files) and reseed.

### 2. Sample data (optional)

With the API running, seed carriers, drivers and loads:

```bash
powershell -ExecutionPolicy Bypass -File scripts/seed-sample-data.ps1
```

The script signs in as the local development admin (see below) and skips seeding if data already exists; pass `-Force` to seed anyway, or `-Email`/`-Password` to sign in as someone else.

### 3. Frontend

```bash
cd frontend
npm install
npm start
# Runs at http://localhost:3000 (calls the API at http://localhost:5050/api)
```

## Accounts and sign-in

Every API endpoint and page requires signing in. There is no public sign-up: administrators create accounts on the **Users** page and share a temporary password, and users can change it via **Change password** in the sidebar.

**The first admin** is created automatically on startup when the database has no users, from the `AdminAccount:Email` and `AdminAccount:Password` settings:

- **Local development:** these come from `backend/TruckLogistics.API/appsettings.Development.json`, so you can sign in straight away with the account defined there. It is for local use only.
- **Shared server:** don't run in Development mode. Set the settings as environment variables before the first start, then sign in and change the password:

  ```bash
  AdminAccount__Email=you@company.com
  AdminAccount__Password=<a strong temporary password>
  ```

  Once an account exists, these settings are ignored and can be removed.

Passwords need at least 6 characters with upper and lower case letters, a number and a symbol. After 5 failed sign-ins an account is locked for 5 minutes. Sign-ins last 8 hours.

## Tests

The API tests start the real API against a private in-memory SQLite database, so they never touch `trucklogistics.db`:

```bash
dotnet test backend/TruckLogistics.sln
```

If the API is already running locally, add `-c Release` so the test build doesn't collide with the running process. CI runs these tests plus a frontend build on every pull request.

## Changing the data model

The schema is managed with EF Core migrations in `backend/TruckLogistics.API/Data/Migrations`. After changing a model or `AppDbContext`, add a migration (stop the running API first):

```bash
dotnet tool restore
dotnet ef migrations add <DescriptiveName> --project backend/TruckLogistics.API --output-dir Data/Migrations
```

The API applies it on next startup. CI fails if the model changes without a matching migration.

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/loads` | List loads (filter: status, search) |
| GET | `/api/loads/{id}` | Load detail with tracking |
| POST | `/api/loads` | Create load |
| PUT | `/api/loads/{id}` | Update load / assign carrier & driver |
| DELETE | `/api/loads/{id}` | Delete load |
| POST | `/api/loads/{id}/tracking` | Add tracking event |
| GET | `/api/loads/stats` | Dashboard stats |
| GET | `/api/carriers` | List carriers |
| POST | `/api/carriers` | Create carrier |
| PUT | `/api/carriers/{id}` | Update carrier |
| GET | `/api/drivers` | List drivers |
| POST | `/api/drivers` | Create driver |
| PATCH | `/api/drivers/{id}/availability` | Toggle availability |
| POST | `/api/auth/login` | Sign in (no token needed); returns a bearer token |
| GET | `/api/auth/me` | Current user's email and admin flag |
| POST | `/api/auth/change-password` | Change own password |
| GET | `/api/users` | List users (admin) |
| POST | `/api/users` | Create user (admin) |
| POST | `/api/users/{id}/reset-password` | Set a new temporary password (admin) |
| DELETE | `/api/users/{id}` | Delete user (admin, not yourself) |

All endpoints except login require an `Authorization: Bearer <accessToken>` header. In Swagger UI, sign in via `POST /api/auth/login`, then paste the `accessToken` into **Authorize**.

## Load Statuses

`Available` → `Booked` → `InTransit` → `Delivered`

## License

MIT
