# Portfolio Blog Backend

The portfolio now has a backend-ready blog architecture:

- `RohitPortfolio.Blazor` — static Blazor WebAssembly frontend
- `RohitPortfolio.Api` — ASP.NET Core Minimal API backend
- `RohitPortfolio.Shared` — shared DTO contracts between frontend and API
- PostgreSQL — production database
- EF Core + Npgsql — database access and migrations
- Markdig — backend Markdown rendering

## Why PostgreSQL

PostgreSQL is the best fit for this portfolio blog because it supports normal relational structure, arrays for tags, strong indexing, future full-text search, and excellent .NET support through the Npgsql EF Core provider.

## Local API setup

Create a local PostgreSQL database:

```powershell
createdb rohit_portfolio_blog
```

The development connection string is in:

```text
RohitPortfolio.Api/appsettings.Development.json
```

Default local value:

```text
Host=localhost;Port=5432;Database=rohit_portfolio_blog;Username=postgres;Password=postgres
```

Change this for your local PostgreSQL user/password.

## Apply migrations

This repo uses a local `dotnet-ef` tool, so first restore tools:

```powershell
dotnet tool restore
```

Then apply the schema:

```powershell
dotnet tool run dotnet-ef database update --project RohitPortfolio.Api/RohitPortfolio.Api.csproj --startup-project RohitPortfolio.Api/RohitPortfolio.Api.csproj
```

## Run the backend

```powershell
dotnet run --project RohitPortfolio.Api/RohitPortfolio.Api.csproj
```

Public endpoints:

```text
GET /api/blog/posts
GET /api/blog/posts/{slug}
GET /api/blog/tags
```

Admin endpoints:

```text
POST   /api/admin/blog/posts
PUT    /api/admin/blog/posts/{id}
DELETE /api/admin/blog/posts/{id}
```

Admin requests must include:

```text
X-Admin-Key: your-admin-key
```

Set the real production key as an environment variable or hosting secret:

```text
Blog__AdminApiKey
```

Do not put the production key in the Blazor frontend.

## Connect the live Blazor site to the API

The Blazor frontend reads:

```text
RohitPortfolio.Blazor/wwwroot/appsettings.json
```

Set:

```json
{
  "BlogApi": {
    "BaseUrl": "https://your-api-host.com"
  }
}
```

Until that value is configured, the frontend shows fallback technical notes so the blog page does not look empty on GitHub Pages.

## Recommended production hosting

Good setup:

- Frontend: GitHub Pages at `rohitsuryaa.com`
- Backend API: Render, Railway, Fly.io, or Azure App Service
- Database: Neon PostgreSQL, Supabase PostgreSQL, Railway PostgreSQL, or Azure Database for PostgreSQL

Use HTTPS for the API and add the production frontend origin to:

```text
Blog:AllowedOrigins
```

The API includes a Dockerfile at:

```text
RohitPortfolio.Api/Dockerfile
```

Required production environment variables:

```text
ConnectionStrings__BlogDb
Blog__AdminApiKey
Blog__AllowedOrigins__0=https://rohitsuryaa.com
```
