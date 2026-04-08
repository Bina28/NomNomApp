# NomNomApp — Plan for Improvement (Junior → Mid-level)

## Phase 1: Middleware (lær middleware pipeline)

### 1.1 Exception Handling Middleware
**Hva:** Lag `server/server/Middleware/ExceptionHandlingMiddleware.cs`

**Steg:**
1. Lag mappen `Middleware/` i server-prosjektet
2. Lag klassen `ExceptionHandlingMiddleware` med constructor som tar `RequestDelegate next` og `ILogger<ExceptionHandlingMiddleware> logger`
3. Lag metoden `InvokeAsync(HttpContext context)`:
   - Wrap `await _next(context)` i try-catch
   - Catch `Exception ex` → logg med `_logger.LogError(ex, "Unhandled exception")`
   - Sett `context.Response.StatusCode = 500`
   - Skriv JSON-respons: `{ "error": "An unexpected error occurred" }` (ikke vis exception-detaljer til brukeren!)
4. Registrer i `Program.cs` **før** alle andre middleware: `app.UseMiddleware<ExceptionHandlingMiddleware>()`
5. Test: Kast en exception i en handler og se at du får pent JSON-svar istedenfor stacktrace

**Læringsmål:** Forstå middleware pipeline, `RequestDelegate`, rekkefølge av middleware

---

### 1.2 Request Logging Middleware
**Hva:** Lag `server/server/Middleware/RequestLoggingMiddleware.cs`

**Steg:**
1. Lag klassen med `RequestDelegate` og `ILogger`
2. I `InvokeAsync`:
   - Lagre `Stopwatch.StartNew()` før `await _next(context)`
   - Etter: logg metode, path, statuskode, tid i ms
   - Bruk structured logging: `_logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms", ...)`
3. Registrer i `Program.cs` etter ExceptionHandling men før auth
4. Kjør appen og se loggene — hvert request skal vises med tid

**Læringsmål:** `Stopwatch`, structured logging med Serilog, performance measurement

---

### 1.3 Correlation ID Middleware
**Hva:** Lag `server/server/Middleware/CorrelationIdMiddleware.cs`

**Steg:**
1. I `InvokeAsync`:
   - Sjekk om request har header `X-Correlation-Id`, ellers lag ny `Guid.NewGuid().ToString()`
   - Legg ID-en i `context.Items["CorrelationId"]`
   - Legg ID-en i response-header: `context.Response.Headers["X-Correlation-Id"] = correlationId`
   - Push til Serilog: `using (LogContext.PushProperty("CorrelationId", correlationId))`
2. Registrer i `Program.cs` som første middleware (før exception handling)
3. Nå vil alle logger for ett request ha samme CorrelationId — nyttig for debugging

**Læringsmål:** Request tracing, headers, `LogContext` i Serilog

---

## Phase 2: Tester (fra 6 til 30+ tester)

### 2.1 Unit-tester for AuthHandler
**Fil:** `server/server.Tests/AuthHandlerTests.cs`

**Tester å skrive (bruk NSubstitute for mocking):**
1. `LoginAsync_ValidCredentials_ReturnsToken` — bruk in-memory DB, legg inn bruker, sjekk at Result er Ok
2. `LoginAsync_WrongPassword_ReturnsFail` — feil passord → Result.Fail
3. `LoginAsync_NonExistentUser_ReturnsFail` — bruker finnes ikke
4. `RegisterAsync_NewUser_ReturnsToken` — ny bruker → lagres i DB + token returneres
5. `RegisterAsync_ExistingEmail_ReturnsFail` — duplikat email
6. `GetCurrentUserAsync_ValidId_ReturnsUserDto` — hent bruker
7. `GetCurrentUserAsync_EmptyId_ReturnsFail` — tom/null ID
8. `GetAllUsersAsync_ExcludesCurrentUser` — sjekk at gjeldende bruker ikke er i listen

**Tips:**
- Se på eksisterende tester i `GetRecipe.cs` for mønster med in-memory DB
- Mock `JwtService` og `PasswordHasher` med NSubstitute
- Husk: `var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb_Auth").Options`
- Bruk unikt DB-navn per test for å unngå data-lekkasje mellom tester

---

### 2.2 Unit-tester for CommentsHandler
**Fil:** `server/server.Tests/CommentsHandlerTests.cs`

**Tester å skrive:**
1. `PostComment_ValidRequest_ReturnsCommentDto` — opprett kommentar, sjekk DTO
2. `PostComment_InvalidRecipeId_ReturnsFail` — ugyldig recipeId-streng
3. `PostComment_UserNotFound_ReturnsFail` — userId som ikke finnes
4. `DeleteComment_ExistingComment_ReturnsTrue` — slett kommentar
5. `DeleteComment_NonExistent_ReturnsFail` — kommentar finnes ikke
6. `GetCommentsForRecipe_ReturnsOrderedByDate` — sjekk sortering
7. `GetCommentsScore_NoComments_ReturnsZero` — tom liste → 0
8. `GetCommentsScore_WithComments_ReturnsAverage` — sjekk gjennomsnitt

**Tips:**
- Mock `SetConnectionManager` med NSubstitute (for SSE broadcast)
- Seed testdata i DB før hver test

---

### 2.3 Integration-tester med WebApplicationFactory
**Fil:** `server/server.Tests/IntegrationTests/AuthIntegrationTests.cs`

**Steg:**
1. Installer NuGet: `Microsoft.AspNetCore.Mvc.Testing`
2. Lag `CustomWebApplicationFactory.cs`:
   - Override `ConfigureWebHost` — bytt ut PostgreSQL med InMemory DB
   - Fjern ekte `DbContext`-registrering, legg inn InMemory
3. Skriv tester som kaller ekte HTTP-endepunkter:
   - `POST /api/auth/register` → 200 + cookie satt
   - `POST /api/auth/login` med feil passord → 401
   - `GET /api/auth/me` uten cookie → 401
   - `GET /api/auth/me` med gyldig cookie → 200 + brukerdata

**Læringsmål:** Integration testing, `HttpClient`, `WebApplicationFactory`, cookie-håndtering i tester

---

### 2.4 Frontend-tester (Vitest + React Testing Library)
**Steg:**
1. Installer: `npm install -D vitest @testing-library/react @testing-library/jest-dom jsdom`
2. Konfigurer `vitest.config.ts`:
   ```ts
   import { defineConfig } from 'vitest/config'
   export default defineConfig({
     test: { environment: 'jsdom', globals: true }
   })
   ```
3. Lag `client/src/__tests__/AuthContext.test.tsx` — test at login setter bruker, logout clearer state
4. Lag `client/src/__tests__/LoginForm.test.tsx` — test at form rendres, submit kaller API

---

## Phase 3: Logging og Observability

### 3.1 Structured logging i alle handlers
**Hva:** Legg til `ILogger<T>` i alle handlers som mangler det

**Filer å endre:**
- `AuthHandler.cs` — legg til logger, logg login/register forsøk (ikke logg passord!)
- `CommentsHandler.cs` — logg opprettelse/sletting av kommentarer
- `FollowsHandler.cs` — logg follow/unfollow
- `CreateRecipeHandler.cs` — logg opprettelse av oppskrifter

**Mønster for logging:**
```csharp
_logger.LogInformation("User {UserId} logged in successfully", userId);
_logger.LogWarning("Failed login attempt for email {Email}", request.Email);
_logger.LogError(ex, "Failed to save recipe {RecipeId}", id);
```

**Viktig:** ALDRI logg passord, tokens, eller personlig info!

---

### 3.2 Serilog Enrichers
**Steg:**
1. Installer NuGet: `Serilog.Enrichers.Environment`, `Serilog.Enrichers.Thread`
2. I `Program.cs` legg til:
   ```csharp
   .Enrich.WithMachineName()
   .Enrich.WithThreadId()
   ```
3. Nå inneholder loggene maskin- og tråd-info — nyttig for debugging

---

## Phase 4: Sikkerhet

### 4.1 Flytt secrets til User Secrets
**Steg:**
1. I server-mappen: `dotnet user-secrets init`
2. `dotnet user-secrets set "JwtSettings:Key" "DIN_HEMMELIGE_NØKKEL"`
3. `dotnet user-secrets set "SpoonacularApi:ApiKey" "DIN_API_KEY"`
4. `dotnet user-secrets set "CloudinarySettings:ApiKey" "DIN_CLOUDINARY_KEY"`
5. `dotnet user-secrets set "CloudinarySettings:ApiSecret" "DIN_CLOUDINARY_SECRET"`
6. `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "DIN_CONNECTION_STRING"`
7. Fjern alle hemmelige verdier fra `appsettings.json` — sett placeholder-verdier
8. Legg til `appsettings.json` tips i README om å bruke User Secrets

**Læringsmål:** Secret management, 12-factor app principles

---

### 4.2 Input Validation med Data Annotations
**Steg:**
1. I `LoginRequest.cs` legg til:
   ```csharp
   [Required, EmailAddress]
   public string Email { get; set; }
   [Required, MinLength(6)]
   public string Password { get; set; }
   ```
2. Gjør det samme for `RegisterRequest`, `CreateCommentRequest`
3. ASP.NET validerer automatisk og returnerer 400 med feilmeldinger

**Neste nivå:** Installer `FluentValidation.AspNetCore` for mer avansert validering

---

### 4.3 Rate Limiting (innebygd i .NET 7+)
**Steg:**
1. I `Program.cs`:
   ```csharp
   builder.Services.AddRateLimiter(options => {
       options.AddFixedWindowLimiter("auth", opt => {
           opt.PermitLimit = 5;
           opt.Window = TimeSpan.FromMinutes(1);
       });
   });
   ```
2. `app.UseRateLimiter()` i pipeline
3. På AuthController: `[EnableRateLimiting("auth")]`

---

## Phase 5: Metrics og Optimization

### 5.1 Health Checks
**Steg:**
1. I `Program.cs`:
   ```csharp
   builder.Services.AddHealthChecks()
       .AddNpgSql(connectionString);  // NuGet: AspNetCore.HealthChecks.NpgSql
   ```
2. `app.MapHealthChecks("/health")`
3. Besøk `/health` — ser du "Healthy"? Da funker DB-tilkoblingen

---

### 5.2 EF Core Optimalisering
**Hva:** Gå gjennom alle DB-kall og optimaliser

**Sjekkliste:**
- [ ] `GetAllUsersAsync` — legg til `.AsNoTracking()` (kun lesing, trenger ikke tracking)
- [ ] `GetCommentsForRecipe` — allerede har `.Select()` projeksjon, bra! Men legg til `.AsNoTracking()`
- [ ] `GetCommentsScore` — **BUG:** henter ALLE kommentarer til minnet, beregner gjennomsnitt i C#. Bruk heller:
  ```csharp
  var avg = await _context.Comments
      .Where(c => c.RecipeId == recipeId)
      .AverageAsync(c => (double?)c.Score) ?? 0;
  ```
  Dette beregner gjennomsnittet i SQL, ikke i minnet!
- [ ] Legg til database-indekser:
  ```csharp
  // I AppDbContext OnModelCreating:
  modelBuilder.Entity<Comment>()
      .HasIndex(c => c.RecipeId);
  modelBuilder.Entity<Follow>()
      .HasIndex(f => f.FollowerId);
  ```

---

### 5.3 Pagination
**Steg:**
1. Lag `PaginationParams.cs` i Shared:
   ```csharp
   public record PaginationParams(int Page = 1, int PageSize = 20);
   public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
   ```
2. Bruk i `GetCommentsForRecipe`:
   ```csharp
   .Skip((page - 1) * pageSize)
   .Take(pageSize)
   ```
3. Bruk i `GetAllUsersAsync` og recipe search

---

### 5.4 Caching
**Steg:**
1. `builder.Services.AddMemoryCache()` i `Program.cs`
2. Inject `IMemoryCache` i `GetRecipeByIdHandler`
3. Sjekk cache før DB:
   ```csharp
   if (_cache.TryGetValue($"recipe_{id}", out RecipeResponse cached))
       return Result<RecipeResponse>.Ok(cached);
   // ... hent fra DB/API ...
   _cache.Set($"recipe_{id}", response, TimeSpan.FromMinutes(30));
   ```

---

## Phase 6: CI/CD

### 6.1 GitHub Actions
**Fil:** `.github/workflows/ci.yml`

```yaml
name: CI
on: [push, pull_request]

jobs:
  backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - run: dotnet restore server/server.sln
      - run: dotnet build server/server.sln --no-restore
      - run: dotnet test server/server.sln --no-build

  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '22'
      - run: cd client && npm ci
      - run: cd client && npm run lint
      - run: cd client && npx vitest run  # når du har tester
```

---

## Sjekkliste — Track progress

- [ ] **Phase 1.1** Exception Handling Middleware
- [ ] **Phase 1.2** Request Logging Middleware
- [ ] **Phase 1.3** Correlation ID Middleware
- [ ] **Phase 2.1** AuthHandler tester (8 tester)
- [ ] **Phase 2.2** CommentsHandler tester (8 tester)
- [ ] **Phase 2.3** Integration-tester med WebApplicationFactory
- [ ] **Phase 2.4** Frontend-tester (Vitest)
- [ ] **Phase 3.1** Structured logging i alle handlers
- [ ] **Phase 3.2** Serilog Enrichers
- [ ] **Phase 4.1** Secrets til User Secrets
- [ ] **Phase 4.2** Input Validation
- [ ] **Phase 4.3** Rate Limiting
- [ ] **Phase 5.1** Health Checks
- [ ] **Phase 5.2** EF Core optimalisering + fiks GetCommentsScore bug
- [ ] **Phase 5.3** Pagination
- [ ] **Phase 5.4** Caching
- [ ] **Phase 6.1** GitHub Actions CI pipeline
