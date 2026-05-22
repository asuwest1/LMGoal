# Software License Tracker

A Windows web application built with ASP.NET Core 8 MVC and Microsoft SQL Server to track software titles, license purchases, maintenance contracts, and subscriptions.

## Features

- **Software Titles** — catalog your software with vendor, category, and website
- **License Purchases** — record perpetual/per-user/site license purchases with cost, quantity, and license keys
- **Maintenance Contracts** — track support/maintenance contracts linked to purchases, with start/end dates
- **Subscriptions** — manage SaaS and subscription licenses with renewal dates and billing periods
- **Dashboard** — at-a-glance view of expiring and expired contracts/subscriptions (60-day warning, 30-day recently expired), plus total annual spend
- **Expiration Alerts** — color-coded status: green (active), yellow (expiring ≤60 days), red (≤14 days), gray (expired)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server (any edition, including SQL Server Express or LocalDB)
- Visual Studio 2022 or VS Code with C# extension

## Setup

### 1. Configure the connection string

Edit `SoftwareTracker/appsettings.json` and update the `DefaultConnection`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SoftwareTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

For a full SQL Server instance:
```
Server=YOUR_SERVER;Database=SoftwareTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true
```

### 2. Run the application

```bash
cd SoftwareTracker
dotnet run
```

The application automatically applies database migrations on startup. The database and tables are created if they don't exist.

### 3. Open in browser

Navigate to `https://localhost:7257` or `http://localhost:5257`

## Database Schema

| Table | Description |
|-------|-------------|
| `SoftwareTitles` | Master list of software products |
| `LicensePurchases` | Individual license purchase records, linked to a software title |
| `MaintenanceContracts` | Support/maintenance contracts with start and end dates, linked to a license purchase |
| `Subscriptions` | Recurring subscriptions with renewal dates, linked to a software title |

## Running Migrations Manually

If you prefer to apply migrations manually instead of auto-apply on startup:

```bash
cd SoftwareTracker
dotnet ef database update
```

## Deployment to IIS

1. Publish: `dotnet publish -c Release -o ./publish`
2. Create an IIS site pointing to the `publish` folder
3. Set the app pool to "No Managed Code"
4. Install the [.NET Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0) on the server
5. Update `appsettings.json` with your production SQL Server connection string
