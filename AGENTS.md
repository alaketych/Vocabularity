# AGENTS.md

## Cursor Cloud specific instructions

### What this repo is
`backend/Vocabularity.sln` is a .NET 8 (ASP.NET Core) vocabulary/dictionary REST API. The
runnable product is `Vocabularity.API`; `Vocabularity.Core` plus the `Vocabularity.Service.*`
projects are class libraries it consumes. The data layer uses EF Core with **SQL Server**
(migrated from Azure Cosmos DB). The other top-level folders (`BlazorApp*`, `WebApplication*`,
`Vocabularity.AuthProvider`, `Vocabularity.Azure*`, `Vocabularity.IdentityServer`, etc.) are
stray VS template scaffolding and are **not** part of `Vocabularity.sln` — ignore them.

### Standard commands (run from `backend/`)
- Restore: `dotnet restore Vocabularity.sln` (the update script already does this on startup)
- Build: `dotnet build Vocabularity.sln -c Debug`
- Lint/format check: `dotnet format Vocabularity.sln --verify-no-changes` (note: the optional
  `Vocabularity.AzureADB2C` project currently has a pre-existing whitespace finding)
- Run the API: `dotnet run --project Vocabularity.API` → listens on `http://localhost:5273`
- There are **no automated test projects** in the repo; "testing" means build + hitting the HTTP
  endpoints (e.g. `POST /language`, `GET /languages`, `POST /user`, `GET /users`, `GET /`).

### SQL Server is required at runtime (non-obvious)
`Vocabularity.API/Program.cs` calls `db.Database.Migrate()` on startup, so the API **crashes on
boot if SQL Server is unreachable**. The committed dev connection string
(`appsettings.Development.json`) points at Windows-only `(localdb)\mssqllocaldb`, which does not
exist on Linux. A SQL Server 2022 Docker container (`vocab-sql`, sa password `Your_password123`,
port 1433) is provisioned in the VM snapshot. Start it and point the API at it:

```bash
# 1. Start the Docker daemon (not auto-started on a fresh session)
sudo dockerd >/tmp/dockerd.log 2>&1 &   # or run in a tmux session
# 2. Start the SQL Server container (persisted in the snapshot)
sudo docker start vocab-sql
# 3. Run the API with the connection string overridden to the container
cd backend
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=Vocabularity;User Id=sa;Password=Your_password123;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet run --project Vocabularity.API
```

If the `vocab-sql` container does not exist (e.g. snapshot not restored), recreate it:
```bash
sudo docker run -d --name vocab-sql -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_password123" \
  -e "MSSQL_PID=Developer" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

### Docker on this VM (non-obvious)
Docker uses the `fuse-overlayfs` storage driver with the containerd snapshotter disabled
(`/etc/docker/daemon.json`) and iptables-legacy, because the Firecracker kernel lacks full
overlay2/nftables support. Always start `dockerd` with `sudo`.
