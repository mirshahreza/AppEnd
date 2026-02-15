# Elsa 3.5.3 Workflow Engine — Integration Plan

> **Branch:** `mohsen-workflow-engine`
> **Date:** 2025-07
> **Status:** Planning
> **Elsa Version:** 3.5.3 (latest stable)
> **Target Framework:** .NET 10

---

## Table of Contents (Split Files)

| File | Content |
|---|---|
| [`ElsaWF-00-Overview.md`](ElsaWF-00-Overview.md) | Project Context, Architecture Decisions |
| [`ElsaWF-01-Phase1-Project.md`](ElsaWF-01-Phase1-Project.md) | Phase 1 — Create AppEndWorkflow Project |
| [`ElsaWF-02-Phase2-ElsaSetup.md`](ElsaWF-02-Phase2-ElsaSetup.md) | Phase 2 — Elsa Services & SQL Server Setup |
| [`ElsaWF-03-Phase3-Integration.md`](ElsaWF-03-Phase3-Integration.md) | Phase 3 — Integration with AppEndHost |
| [`ElsaWF-04-Phase4-Samples.md`](ElsaWF-04-Phase4-Samples.md) | Phase 4 — Sample Workflows (4 workflows) |
| [`ElsaWF-05-Phase5-Verify.md`](ElsaWF-05-Phase5-Verify.md) | Phase 5 — Build & Verify |
| [`ElsaWF-06-Phase6-UI.md`](ElsaWF-06-Phase6-UI.md) | Phase 6 — Vue.js Workflow Management UI |
| [`ElsaWF-07-Phase7-Activities.md`](ElsaWF-07-Phase7-Activities.md) | Phase 7 — Custom Activity Library (48 activities) |
| [`ElsaWF-08-Structure-Notes.md`](ElsaWF-08-Structure-Notes.md) | File Structure, Dependency Graph, Key Notes, Execution Order |

---

## Project Context

### Existing Solution Structure

| Project | Type | Role |
|---|---|---|
| `AppEndCommon` | Class Library | Settings (`AppEndSettings`), extensions, logging, models |
| `AppEndDynaCode` | Class Library | Dynamic code compilation (Roslyn) |
| `AppEndDbIO` | Class Library | DB access, `DbConf`, `DbDialog` |
| `AppEndServer` | Class Library (Web SDK) | Services, RPC middleware, scheduling |
| `AppEndHost` | Web App | Entry point, `Program.cs`, static files, workspace |

### Existing Patterns (Must Follow)

- **RPC Pattern:** Vue.js calls `rpcAEP("MethodName", {inputs}, callback)` → hits `Zzz.AppEndProxy.MethodName` → calls static C# methods
- **Settings:** `AppEndSettings` reads from `appsettings.json` / `appsettings.Development.json` → custom JSON structure (not standard `IConfiguration`)
- **DB Config:** `AppEnd.DbServers[]` array of `{Name, ServerType, ConnectionString}` → accessed via `DbConf.FromSettings(name)`
- **Default DB:** `AppEndSettings.DefaultDbConfName` → returns the default SQL Server connection
- **Vue Components:** Follow pattern in `AppEndStudio/components/` — card layout, `rpcAEP` calls, `_this` pattern, Bootstrap 5 + FontAwesome
- **Styles:** Global in `AppEndStudio/assets/custom.css` or `a..lib/append/css/`
- **Scheduler:** Custom `SchedulerService` (BackgroundService + Cron) — NOT touched by this integration

### Connection String Access

```
AppEndSettings.DefaultDbConfName  →  "DefaultRepo" (or configured name)
       ↓
DbConf.FromSettings("DefaultRepo")
       ↓
dbConf.ConnectionString  →  "Data Source=...;Initial Catalog=...;..."
       ↓
Elsa UseSqlServer(connectionString)
```

---

## Architecture Decisions

| Decision | Choice | Rationale |
|---|---|---|
| **Persistence** | SQL Server (existing default DB) | No new database; Elsa tables created manually via SQL scripts in separate DB project |
| **UI Framework** | Vue.js + jQuery (existing stack) | No Blazor; Elsa Studio excluded entirely |
| **UI Communication** | `rpcAEP` → C# bridge → Elsa SDK | Consistent with existing RPC pattern; auth/CORS handled by existing pipeline |
| **API Exposure** | None — RPC only | Elsa REST API (`UseWorkflowsApi`) is NOT enabled. No new HTTP routes. All operations go through the single `talk-to-me` endpoint |
| **Workflow Triggers** | RPC-only (no HTTP triggers) | `UseHttp()` is NOT enabled. Workflows are triggered programmatically via `WorkflowServices.ExecuteWorkflow()` through RPC |
| **Encapsulation** | New `AppEndWorkflow` project | Isolates Elsa dependency; clean separation |
| **Existing Scheduler** | Untouched | Elsa workflows and existing scheduler coexist independently |
