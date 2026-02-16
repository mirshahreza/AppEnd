# Phase 2 — Elsa Services & SQL Server Setup

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## Goal
Create extension methods that configure Elsa with SQL Server persistence using the existing default connection string.

## File: `AppEndWorkflow/ElsaSetup.cs`

Two extension methods:

### `AddAppEndWorkflow(this IServiceCollection services)`
- Read connection string: `DbConf.FromSettings(AppEndSettings.DefaultDbConfName).ConnectionString`
- Call `services.AddElsa(elsa => { ... })` with:
  - **Management store:** `elsa.UseWorkflowManagement(m => m.UseSqlServer(connectionString))`
  - **Runtime store:** `elsa.UseWorkflowRuntime(r => r.UseSqlServer(connectionString))`
  - **JavaScript:** `elsa.UseJavaScript()` — scripting in workflows
  - **Labels:** `elsa.UseLabels(l => l.UseSqlServer(connectionString))`
- **NOT included:**
  - ~~`elsa.UseWorkflowsApi()`~~ — No separate REST endpoints; all access through `rpcAEP` bridge
  - ~~`elsa.UseHttp()`~~ — No HTTP trigger activities; workflows are triggered via RPC only
  - ~~`elsa.UseIdentity()`~~ — Auth is handled by AppEnd's existing pipeline

### `UseAppEndWorkflow(this WebApplication app)`
- Call `app.UseWorkflows()` — Elsa internal middleware (timer triggers, bookmarks processing)
- **NOT included:**
  - ~~`app.UseWorkflowsApi()`~~ — No separate API routes; everything goes through `talk-to-me` RPC endpoint

## DB Tables
Elsa auto-migration is **disabled**. Tables are NOT created automatically.

Instead, a SQL setup script (`ElsaSchema.sql`) will be generated and provided to be placed
in the **separate database setup project** that the team manages independently.

The script will create the following tables (in `Elsa` schema):
- `Elsa.WorkflowDefinitions`
- `Elsa.WorkflowInstances`
- `Elsa.Bookmarks`
- `Elsa.WorkflowExecutionLogRecords`
- `Elsa.Labels`
- `Elsa.WorkflowDefinitionLabels`
- etc.

**How auto-migration is disabled:**
In `ElsaSetup.cs`, the `UseSqlServer()` call will include `options.DisableAutoMigrations = true`
(or equivalent Elsa 3.x configuration) so that no schema changes happen at runtime.

**Workflow:**
1. During Phase 2 implementation, extract the full SQL script from Elsa EF Core migrations
2. Deliver the script as `ElsaSchema.sql`
3. User places it in their DB setup project and runs it manually before first app start

All tables are created in the **same database** as the existing application data.
