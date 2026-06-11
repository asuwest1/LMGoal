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

## Deployment to Windows Server / IIS

This walkthrough is written for a first-time deployment from this GitHub repo to a Windows Server running IIS. Each step says **where** to run it: your **local/dev machine** or the **Windows Server**.

### Prerequisites on the Windows Server

1. **Install IIS** (if not already installed) — *on the Windows Server*
   - Open **Server Manager** → **Add Roles and Features**
   - Select **Web Server (IIS)** role → finish the wizard with defaults

2. **Install the .NET 8 Hosting Bundle** — *on the Windows Server*
   - Download the "Hosting Bundle" installer from https://dotnet.microsoft.com/download/dotnet/8.0
   - Run the downloaded `.exe` installer
   - Restart the server (or at least run `net stop was /y` then `net start w3svc` in an elevated PowerShell/cmd prompt) so IIS picks up the new module

3. **Install SQL Server** (if not already available) — *on the Windows Server (or a separate DB server)*
   - SQL Server Express is sufficient for small deployments: https://www.microsoft.com/sql-server/sql-server-downloads
   - During setup, note the instance name (e.g., `SQLEXPRESS`) — you'll need it for the connection string

### Step 1: Get the code onto your local machine

*Run on your local/dev machine, in a terminal (PowerShell, Command Prompt, or Git Bash):*

```bash
git clone https://github.com/asuwest1/lmgoal.git
cd lmgoal
```

If you don't have Git installed, you can instead go to the GitHub repo page, click **Code → Download ZIP**, and extract it.

### Step 2: Install the .NET 8 SDK locally

*On your local/dev machine:*

- Download and install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (the SDK, not just the runtime)
- Verify it installed correctly:

```bash
dotnet --version
```

You should see a version starting with `8.`.

### Step 3: Set the production database connection string

*On your local/dev machine, edit a file:*

Open `SoftwareTracker/appsettings.json` in a text editor and update `DefaultConnection` to point at your production SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME\\SQLEXPRESS;Database=SoftwareTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Replace `YOUR_SERVER_NAME\SQLEXPRESS` with your actual SQL Server name/instance. If you're using SQL authentication instead of Windows authentication, use:

```
Server=YOUR_SERVER_NAME;Database=SoftwareTrackerDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=true
```

> Tip: It's safer to set this connection string directly in `appsettings.Production.json` on the server (after publishing) rather than committing production credentials to the repo. If you go that route, leave `appsettings.json` as-is and create/edit `appsettings.Production.json` in the published folder on the server in Step 6.

### Step 4: Publish the application

*On your local/dev machine, in a terminal:*

```bash
cd SoftwareTracker
dotnet publish -c Release -o ./publish
```

This compiles the app and places all the files IIS needs into `SoftwareTracker/publish`. This `publish` folder is everything you need to copy to the server.

### Step 5: Copy the published files to the server

*From your local/dev machine to the Windows Server:*

- Zip the `SoftwareTracker/publish` folder (right-click → "Send to" → "Compressed (zipped) folder")
- Copy the zip to the Windows Server using whichever method you have available:
  - Remote Desktop (RDP) — copy/paste the file or drag-and-drop into the RDP session
  - A network share
  - SCP/SFTP if enabled

*On the Windows Server:*

- Extract the zip to a folder where the app will live, e.g.:
  ```
  C:\inetpub\wwwroot\SoftwareTracker
  ```

### Step 6: (Optional) Set the connection string on the server instead

*On the Windows Server, if you skipped editing it in Step 3:*

Create a file named `appsettings.Production.json` in `C:\inetpub\wwwroot\SoftwareTracker` next to `appsettings.json`, with:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME\\SQLEXPRESS;Database=SoftwareTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

This file overrides settings in `appsettings.json` when the app runs in Production mode (the IIS default), so you don't need to put production credentials in the repo.

### Step 7: Create the IIS Application Pool

*On the Windows Server, open IIS Manager (search for "IIS" in the Start menu):*

1. In the left tree, right-click **Application Pools** → **Add Application Pool**
2. Name it `SoftwareTrackerPool`
3. Set **.NET CLR Version** to **No Managed Code**
4. Leave **Managed pipeline mode** as **Integrated**
5. Click **OK**

### Step 8: Create the IIS Site

*On the Windows Server, in IIS Manager:*

1. Right-click **Sites** → **Add Website**
2. Fill in:
   - **Site name**: `SoftwareTracker`
   - **Application pool**: select `SoftwareTrackerPool` (created in Step 7)
   - **Physical path**: `C:\inetpub\wwwroot\SoftwareTracker`
   - **Binding**: choose a port (e.g., `80` for HTTP, or `443` with an SSL certificate for HTTPS) and optionally a hostname
3. Click **OK**

### Step 9: Set folder permissions

*On the Windows Server:*

1. Right-click the `C:\inetpub\wwwroot\SoftwareTracker` folder → **Properties** → **Security** tab
2. Click **Edit** → **Add**
3. Add the user `IIS AppPool\SoftwareTrackerPool` (this is the identity your app pool runs as)
4. Grant it **Read & execute** permissions (and **Modify** if the app needs to write logs/files to this folder)
5. Click **OK**

### Step 10: Allow the site through the firewall (if needed)

*On the Windows Server, in an elevated PowerShell prompt:*

```powershell
New-NetFirewallRule -DisplayName "SoftwareTracker HTTP" -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow
```

(Use port 443 instead if you configured HTTPS.)

### Step 11: Browse to the site

*From any machine on the network, in a web browser:*

```
http://your-server-name-or-ip/
```

(or `https://...` if you configured HTTPS)

The app applies database migrations automatically on first startup, creating `SoftwareTrackerDb` and its tables if they don't already exist.

### Updating the app later

When you push new changes to the repo, repeat **Steps 1, 4, and 5**:
1. `git pull` (local machine, in the `lmgoal` folder)
2. `dotnet publish -c Release -o ./publish` (local machine, in `SoftwareTracker`)
3. Copy the new `publish` contents over the files in `C:\inetpub\wwwroot\SoftwareTracker` (server)

Before overwriting, it's a good idea to:
- *On the Windows Server, in IIS Manager:* select the site → click **Stop** in the Actions pane
- Copy the new files
- Click **Start** again

### Troubleshooting

- **HTTP Error 500.19 or 502.5**: The .NET Hosting Bundle isn't installed, or IIS hasn't been restarted since installing it. Re-run the hosting bundle installer and restart the server.
- **Database connection errors**: Double-check the connection string in `appsettings.json`/`appsettings.Production.json` and confirm the app pool identity (`IIS AppPool\SoftwareTrackerPool`) has access to the SQL Server (for Windows Authentication, you may need to grant this account login rights in SQL Server).
- **"Access to the path is denied"**: Revisit Step 9 and confirm folder permissions for the app pool identity.
