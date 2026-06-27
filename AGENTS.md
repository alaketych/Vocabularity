# Vocabularity

A .NET 7.0 solution containing three minimal ASP.NET Core web apps, each currently exposing a single `GET /` endpoint that returns `Hello World!`:

- `Vocabularity.API` — API service (in the solution `Vocabularity.sln`).
- `Vocabularity.IdentityServer` — IdentityServer4-based auth service (in the solution).
- `Vocabularity` — standalone web app project (NOT referenced by `Vocabularity.sln`; build it directly from its own folder).

## Cursor Cloud specific instructions

- The SDK is **.NET 7.0** (`net7.0`), installed at `/usr/share/dotnet` with a `dotnet` symlink in `/usr/local/bin`. .NET 7 is not available via apt on this image; it is provided via the dotnet-install script during environment setup (the update script only refreshes NuGet packages with `dotnet restore`).
- Build/lint check: `dotnet build` from the repo root builds the two solution projects. There is no separate linter and there are no automated tests in this repo. Build `Vocabularity` separately (`cd Vocabularity && dotnet build`) since it is not in the solution.
- Run a service in dev mode, e.g. `cd Vocabularity.API && ASPNETCORE_ENVIRONMENT=Development dotnet run --launch-profile http`. Default HTTP dev ports (from `Properties/launchSettings.json`): `Vocabularity.API` → 5051, `Vocabularity.IdentityServer` → 5273, `Vocabularity` → 5235. Verify with `curl http://localhost:<port>/` (expect `Hello World!`).
- The `https` launch profiles rely on the ASP.NET Core dev certificate; prefer the `http` profile in this headless environment to avoid TLS trust issues.
