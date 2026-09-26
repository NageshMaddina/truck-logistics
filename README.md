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
│   └── TruckLogistics.API/     # ASP.NET Core Web API
│       ├── Controllers/        # Loads, Carriers, Drivers endpoints
│       ├── Models/             # EF Core entities
│       ├── Data/               # DbContext
│       └── DTOs/               # Request/response shapes
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

On first run in Development mode the API creates `trucklogistics.db` (via `EnsureCreated`). The database file is git-ignored; delete it to start fresh.

### 2. Sample data (optional)

With the API running, seed carriers, drivers and loads:

```bash
powershell -ExecutionPolicy Bypass -File scripts/seed-sample-data.ps1
```

The script skips seeding if data already exists; pass `-Force` to seed anyway.

### 3. Frontend

```bash
cd frontend
npm install
npm start
# Runs at http://localhost:3000 (calls the API at http://localhost:5050/api)
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
