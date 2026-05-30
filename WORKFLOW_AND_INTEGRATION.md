# BikeRental.Modern - Workflow and System Integration

## Purpose
This document describes:
- The current day-to-day workflow used in this project
- How the current solution integrates into a larger enterprise system

## 1. Current Workflow

### 1.1 Development workflow
1. Start backend API from `BikeRental.API`:
   - `dotnet run`
2. Start frontend app from `bikrental-ui`:
   - `npm install --legacy-peer-deps` (first time only)
   - `npm run dev`
3. Open UI at `http://localhost:5173`
4. Validate API health at `http://localhost:5035/health`
5. Test core flow:
   - Browse beach/mountain bikes
   - Rent bike
   - Add accessories in modal
   - Verify pricing and discount behavior

### 1.2 Runtime request workflow
1. User action in React UI triggers typed API call (`src/services/api.ts`)
2. Request reaches ASP.NET Core controller
3. Controller delegates to service layer (`IBikeService` / `IAccessoryService`)
4. Service accesses EF Core `BikeRentalDbContext`
5. SQL Server LocalDB stores/reads data
6. Response DTO returned to UI
7. UI updates state and feedback (loading, toast, modal)

### 1.3 Error handling workflow
- Backend exceptions are handled by global exception middleware
- Middleware logs error and returns consistent JSON response
- Frontend fetch wrapper surfaces readable errors to UI components

### 1.4 Configuration workflow
- Backend config from `appsettings.json`:
  - `ConnectionStrings:DefaultConnection`
  - `AllowedOrigins`
  - `FleetSettings`
- Frontend config from `.env`:
   - `REACT_APP_API_URL`

## 2. Integration into a Larger System

### 2.1 API integration model
`BikeRental.API` can be integrated as a domain microservice behind an API gateway.

Integration points:
- Gateway routing to `/api/*`
- Health probe via `/health`
- Config-based CORS for approved clients
- Service interfaces allow extension for external systems

### 2.2 UI integration model
`bikrental-ui` can be integrated as:
- Standalone SPA on static hosting/CDN, or
- Embedded module in a larger portal shell

Integration controls:
- API endpoint is environment-driven (`REACT_APP_API_URL`)
- No backend coupling in UI deployment pipeline

### 2.3 Data and platform integration
- DB layer currently uses SQL Server LocalDB (dev)
- For larger systems, switch connection string to managed SQL instance
- Keep `FleetSettings` environment-specific for business defaults
- Inject secrets/config through CI/CD or secret manager

### 2.4 Enterprise readiness path
For larger production rollout, add:
- Authentication/authorization (OIDC/JWT)
- API versioning
- Rate limiting
- Structured observability (tracing, metrics, dashboards)
- Environment promotion via CI/CD (dev/test/stage/prod)

## 3. Verification Checklist

### Workflow verification
- [ ] Backend starts without port conflicts
- [ ] Frontend starts and loads bike lists
- [ ] Rent operation updates availability correctly
- [ ] Accessory order and discount logic works
- [ ] Health endpoint returns success

### Integration readiness verification
- [ ] API URL is configurable per environment
- [ ] AllowedOrigins configured for target client hosts
- [ ] Connection string externalized from code
- [ ] Error responses are JSON and consistent
- [ ] Service boundaries are respected (controllers do not contain business logic)

## 4. Notes
- This document is intentionally workflow-focused and separate from the root verification README.
- Root verification details remain in `README.md`; this file is the operational integration companion.
