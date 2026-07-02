# TruckLogix — Truck Logistics Management System

A TQL-style freight brokerage management application built with React.js, C# .NET 8, and SQL Server.

## Tech Stack

- **Frontend**: React.js 18 with React Router v6
- **Backend**: ASP.NET Core 8 Web API with Entity Framework Core
- **Database**: SQL Server with EF Core migrations

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
│   └── TruckLogistics.API/     # ASP.NET Core Web API
│       ├── Controllers/        # Loads, Carriers, Drivers endpoints
│       ├── Models/             # EF Core entities
│       ├── Data/               # DbContext
│       └── DTOs/               # Request/response shapes
├── frontend/
│   └── src/
│       ├── pages/              # Dashboard, Loads, Carriers, Drivers
│       └── services/           # Axios API client
└── database/
    └── schema.sql              # SQL Server DDL + seed data
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB works fine)

### 1. Database

Run `database/schema.sql` in SQL Server Management Studio or `sqlcmd`:

```bash
sqlcmd -S localhost -i database/schema.sql
```

Or let EF Core create the DB automatically on first run (the API calls `EnsureCreated` in dev mode).

### 2. Backend

```bash
cd backend/TruckLogistics.API
# Update connection string in appsettings.json if needed
dotnet restore
dotnet run
# API runs at https://localhost:7000 / http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### 3. Frontend

```bash
cd frontend
npm install
npm start
# Runs at http://localhost:3000
```

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

## Load Statuses

`Available` → `Booked` → `InTransit` → `Delivered`

## License

MIT
