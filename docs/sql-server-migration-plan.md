# SQL Server Migration Plan

## Feasibility Assessment

**Verdict: Highly feasible — low risk, moderate effort (~2–3 days)**

The project uses **EF Core 9.0.6**, which has first-class support for SQL Server via Microsoft's official provider. Since no MySQL-specific features are used (no JSON columns, arrays, stored procedures, or full-text search), the migration is mostly a provider swap with configuration updates.

---

## Current Stack

| Component       | Current                                |
|-----------------|----------------------------------------|
| Database        | MySQL 8.0 (local) / Aiven MySQL (prod) |
| ORM             | EF Core 9.0.6                          |
| MySQL Driver    | `Pomelo.EntityFrameworkCore.MySql` 9.0 |
| Backend         | ASP.NET Core 10 / C#                   |
| Architecture    | Clean Architecture (Domain → App → Infrastructure → API) |
| Entities        | 32 entities, 21 Fluent API configs      |

---

## Risk Analysis

| Risk | Severity | Notes |
|------|----------|-------|
| MySQL-specific data types | Low | Only uses standard types (VARCHAR, TEXT, DECIMAL, DATETIME, INT) |
| Character encoding (utf8mb4) | Low | SQL Server supports Unicode natively via `nvarchar` — EF Core handles this |
| AUTO_INCREMENT vs IDENTITY | None | EF Core abstracts both identically |
| Cascade delete rules | None | Configured in Fluent API, provider-agnostic |
| Seed data SQL script | Medium | `docs/seed_data.sql` uses MySQL syntax — needs rewrite |
| Existing migrations | Low | Drop and regenerate; no production data at risk currently |
| Docker setup | Low | Replace MySQL image with SQL Server image |

---

## Step-by-Step Migration Plan

### Phase 1 — Package Swap (30 min)

**File:** `src/UniversityPortal.Infrastructure/UniversityPortal.Infrastructure.csproj`

1. Remove the Pomelo MySQL package:
   ```xml
   <!-- Remove this -->
   <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.0" />
   ```

2. Add the SQL Server provider:
   ```xml
   <!-- Add this -->
   <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.6" />
   ```

3. Run:
   ```bash
   dotnet restore
   ```

---

### Phase 2 — Update DbContext Registration (30 min)

**File:** `src/UniversityPortal.Infrastructure/DependencyInjection.cs`

Replace the Pomelo MySQL setup:
```csharp
// Before
services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// After
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

Update the using statement:
```csharp
// Before
using Pomelo.EntityFrameworkCore.MySql...

// After
using Microsoft.EntityFrameworkCore;
```

---

### Phase 3 — Update Entity Configurations (1–2 hours)

**Directory:** `src/UniversityPortal.Infrastructure/Persistence/Configurations/`

Audit all 21 configuration files for MySQL-specific directives and remove them.

**Common patterns to find and remove:**

```csharp
// Remove charset/collation annotations (MySQL-only)
entity.HasCharSet("utf8mb4");
entity.UseCollation("utf8mb4_unicode_ci");

// Remove MySQL column type overrides if any
// .HasColumnType("longtext")  →  .HasColumnType("nvarchar(max)")
// .HasColumnType("tinyint(1)") →  EF Core maps bool automatically
```

SQL Server equivalents EF Core handles automatically:
- `VARCHAR` → `nvarchar` (Unicode by default)
- `TEXT` → `nvarchar(max)`
- `DATETIME(6)` → `datetime2`
- `TINYINT(1)` bool → `bit`
- `AUTO_INCREMENT` → `IDENTITY(1,1)`

---

### Phase 4 — Regenerate Migrations (30 min)

The existing migration was generated for MySQL and is incompatible. Delete and regenerate:

```bash
# Delete old MySQL migrations
rm -rf src/UniversityPortal.Infrastructure/Persistence/Migrations/

# Generate new SQL Server migration
dotnet ef migrations add InitialCreate \
  --project src/UniversityPortal.Infrastructure \
  --startup-project src/UniversityPortal.API

# Apply to database
dotnet ef database update \
  --project src/UniversityPortal.Infrastructure \
  --startup-project src/UniversityPortal.API
```

---

### Phase 5 — Update Connection Strings (15 min)

**File:** `src/UniversityPortal.API/appsettings.json`

```json
// Before (MySQL)
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=university_portal;User=root;Password=password;"
}

// After (SQL Server)
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=UniversityPortal;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
}
```

**File:** `src/UniversityPortal.API/appsettings.Development.json`

Update the Aiven/production URL to point to the new SQL Server instance.

Also update `.env.example` to reflect the new connection string format.

---

### Phase 6 — Update Docker Compose (30 min)

**File:** `docker-compose.yml`

```yaml
# Before
db:
  image: mysql:8.0
  environment:
    MYSQL_ROOT_PASSWORD: password
    MYSQL_DATABASE: university_portal
  ports:
    - "3306:3306"

# After
db:
  image: mcr.microsoft.com/mssql/server:2022-latest
  environment:
    SA_PASSWORD: "YourStrong@Passw0rd"
    ACCEPT_EULA: "Y"
    MSSQL_PID: Developer
  ports:
    - "1433:1433"
```

---

### Phase 7 — Rewrite Seed Data Script (1 hour)

**File:** `docs/seed_data.sql`

The existing seed script uses MySQL syntax. Rewrite for T-SQL (SQL Server):

| MySQL syntax | T-SQL equivalent |
|---|---|
| `` `backtick` `` table names | `[bracket]` or plain names |
| `AUTO_INCREMENT` | `IDENTITY(1,1)` |
| `SET NAMES utf8mb4` | Remove (not needed) |
| `NOW()` | `GETDATE()` |
| `TRUE` / `FALSE` | `1` / `0` |
| `LIMIT n` | `TOP n` or `FETCH NEXT n ROWS ONLY` |

Alternatively, convert the seed data into EF Core `HasData()` seeding inside configurations — this is provider-agnostic and the recommended approach.

---

### Phase 8 — Test & Verify (2–4 hours)

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Start the application
dotnet run --project src/UniversityPortal.API

# Verify Swagger UI loads at http://localhost:<port>/swagger
# Test key API endpoints:
#   POST /api/auth/login
#   GET  /api/sinhvien
#   GET  /api/monhoc
```

Checklist:
- [ ] All 32 entities created correctly in SQL Server
- [ ] Foreign key constraints applied
- [ ] Seed data inserted successfully
- [ ] Authentication (JWT) works
- [ ] CRUD operations work on all main entities
- [ ] No runtime EF Core errors

---

## Effort Estimate

| Phase | Task | Est. Time |
|-------|------|-----------|
| 1 | Package swap | 30 min |
| 2 | DI configuration update | 30 min |
| 3 | Entity configuration audit (21 files) | 1–2 hours |
| 4 | Regenerate migrations | 30 min |
| 5 | Connection strings | 15 min |
| 6 | Docker Compose | 30 min |
| 7 | Rewrite seed SQL | 1 hour |
| 8 | Test & verify | 2–4 hours |
| **Total** | | **~2–3 days** |

---

## What Does NOT Need to Change

- All 32 domain entity classes — zero changes
- All application layer (use cases, DTOs, interfaces) — zero changes
- All API controllers — zero changes
- Repository pattern implementation — zero changes
- JWT authentication logic — zero changes
- Clean Architecture structure — zero changes

EF Core's abstraction layer means **~85% of the codebase is untouched**.

---

## Recommended SQL Server Edition for Dev/Local

Use **SQL Server 2022 Developer Edition** (free for dev/test):
```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest
```

For production, consider **Azure SQL Database** — it is the cloud-native SQL Server offering and integrates well with the existing Aiven-style cloud DB pattern already in use.
