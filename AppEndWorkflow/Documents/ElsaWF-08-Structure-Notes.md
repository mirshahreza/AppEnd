# File Structure, Dependency Graph & Key Notes

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## File Structure Summary

### New Files

```
AppEndWorkflow/                              ← NEW PROJECT
├── AppEndWorkflow.csproj
├── ElsaSetup.cs                             ← AddAppEndWorkflow() + UseAppEndWorkflow()
├── WorkflowServices.cs                      ← Bridge: rpcAEP → Elsa SDK (static methods)
├── ElsaSchema.sql                           ← SQL script for DB setup project (manual)
├── Activities/                              ← CUSTOM ACTIVITY LIBRARY (Phase 7) — 48 activities
│   ├── Notifications/
│   │   ├── SendEmailActivity.cs             ← Send email via SMTP
│   │   ├── SendSmsActivity.cs               ← Send SMS via provider API
│   │   ├── SendTelegramActivity.cs          ← Telegram message via Bot API
│   │   └── SendPushNotificationActivity.cs  ← Push notification to user
│   ├── Database/
│   │   ├── RunSqlQueryActivity.cs           ← Execute SELECT, return JSON
│   │   └── RunSqlCommandActivity.cs         ← Execute INSERT/UPDATE/DELETE
│   ├── AppEndIntegration/
│   │   ├── CallRpcMethodActivity.cs         ← Call existing RPC methods
│   │   └── WriteLogActivity.cs              ← Write to AppEnd logging
│   ├── HumanTasks/
│   │   ├── AssignToUserActivity.cs          ← Assign task to user/role
│   │   └── WaitForApprovalActivity.cs       ← Suspend, wait for approval
│   ├── DataDocuments/
│   │   ├── GeneratePdfActivity.cs           ← Generate PDF from template
│   │   ├── GenerateExcelActivity.cs         ← Generate Excel from JSON
│   │   ├── TransformJsonActivity.cs         ← Transform/map JSON
│   │   ├── ValidateDataActivity.cs          ← Validate data against rules
│   │   └── MergeJsonActivity.cs             ← Deep-merge JSON objects
│   ├── Http/
│   │   ├── CallHttpApiActivity.cs           ← Call REST API endpoint
│   │   ├── CallSoapServiceActivity.cs       ← Call SOAP web service
│   │   └── DownloadFileActivity.cs          ← Download file from URL
│   ├── FileSystem/
│   │   ├── ReadFileActivity.cs              ← Read file content
│   │   ├── WriteFileActivity.cs             ← Write content to file
│   │   ├── CopyMoveFileActivity.cs          ← Copy or move file
│   │   ├── DeleteFileActivity.cs            ← Delete file
│   │   └── ListFilesActivity.cs             ← List files in directory
│   ├── Text/
│   │   ├── RenderTemplateActivity.cs        ← Render text template
│   │   ├── RegexMatchActivity.cs            ← Regex pattern matching
│   │   ├── FormatStringActivity.cs          ← Format string with culture
│   │   └── ParseCsvActivity.cs              ← Parse CSV to JSON
│   ├── Security/
│   │   ├── HashDataActivity.cs              ← Hash (SHA256/SHA512/MD5)
│   │   ├── EncryptDecryptActivity.cs        ← AES encrypt/decrypt
│   │   ├── GenerateTokenActivity.cs         ← Generate GUID/random/JWT
│   │   └── CheckPermissionActivity.cs       ← Check user permissions
│   ├── Collections/
│   │   ├── FilterArrayActivity.cs           ← Filter JSON array
│   │   ├── SortArrayActivity.cs             ← Sort JSON array
│   │   ├── AggregateArrayActivity.cs        ← Sum/Avg/Min/Max/Count
│   │   ├── GroupByActivity.cs               ← Group array by field
│   │   └── PickFromArrayActivity.cs         ← Pick first/last/random
│   ├── FlowControl/
│   │   ├── DelayActivity.cs                 ← Pause for duration
│   │   ├── WaitForSignalActivity.cs         ← Wait for external signal
│   │   ├── ParallelForEachActivity.cs       ← Parallel iteration
│   │   ├── RetryActivity.cs                 ← Retry with backoff
│   │   └── SwitchActivity.cs                ← Multi-branch switch
│   ├── Archive/
│   │   ├── CompressFilesActivity.cs         ← Create ZIP archive
│   │   └── DecompressFilesActivity.cs       ← Extract ZIP archive
│   ├── Math/
│   │   ├── EvaluateExpressionActivity.cs    ← Evaluate math expression
│   │   └── ConvertCurrencyActivity.cs       ← Currency conversion
│   └── Cache/
│       ├── SetCacheActivity.cs              ← Store value in cache
│       ├── GetCacheActivity.cs              ← Retrieve from cache
│       └── RemoveCacheActivity.cs           ← Remove cache entries
└── Workflows/
    ├── HelloWorldWorkflow.cs                ← Sample: basic verification
    ├── ScheduledDbCheckWorkflow.cs          ← Sample: timer/cron + DB query
    ├── OrderApprovalWorkflow.cs             ← Sample: human task + kartabl
    └── DataPipelineWorkflow.cs              ← Sample: batch processing + error handling

AppEndStudio/components/                     ← ADMIN COMPONENTS
├── WorkflowDefinitions.vue                  ← CRUD for workflow definitions
├── WorkflowInstances.vue                    ← Monitoring execution instances
├── WorkflowInstanceDetail.vue               ← Single instance detail + execution log
└── WorkflowActivityBrowser.vue              ← Activity type reference browser

a.SharedComponents/                          ← USER-FACING COMPONENT
└── WorkflowInbox.vue                        ← Kartabl: task inbox for all users

a..lib/append/css/                           ← STYLES
└── append-workflow.css                      ← Dedicated workflow styles
```

### Modified Files

```
AppEnd.sln                                   ← Add AppEndWorkflow project
AppEndHost/AppEndHost.csproj                 ← Add ProjectReference to AppEndWorkflow
AppEndHost/Program.cs                        ← Add AddAppEndWorkflow() + UseAppEndWorkflow()
```

---

## Dependency Graph

```
AppEndHost
├── AppEndServer
│   ├── AppEndDbIO ──→ AppEndCommon
│   │                → AppEndDynaCode → AppEndCommon
│   ├── AppEndCommon
│   └── AppEndDynaCode
├── AppEndWorkflow  ← [NEW]
│   ├── AppEndCommon     (AppEndSettings, logging)
│   └── AppEndDbIO       (DbConf.FromSettings → connection string)
├── AppEndCommon
├── AppEndDbIO
└── AppEndDynaCode
```

---

## Key Notes & Constraints

1. **No Blazor**
2. **No SQLite** — SQL Server only, using the existing default database connection.
3. **No new database** — Elsa tables are created in the same DB as existing application tables.
4. **No changes to SchedulerService** — Existing scheduler and Elsa coexist independently.
5. **No auto-migration** — Elsa EF Core auto-migration is disabled. A SQL setup script (`ElsaSchema.sql`) is provided for the separate DB setup project. Tables must be created manually before first app start.
6. **RPC bridge pattern** — Vue.js never calls Elsa API directly; always through `rpcAEP` → `WorkflowServices.cs` → Elsa SDK.
7. **No separate HTTP routes** — Elsa does NOT expose any REST API endpoints or HTTP triggers. All operations go through the single `talk-to-me` RPC endpoint. `UseWorkflowsApi()` and `UseHttp()` are NOT used.
8. **Visual workflow designer** (drag & drop) is out of scope for this plan. Can be added later using `vue-flow`, `drawflow`, or `rete.js`.
9. **NuGet packages:** `Elsa` 3.5.3 + `Elsa.EntityFrameworkCore.SqlServer` 3.5.3.
10. **Workflow triggers** — Workflows are triggered exclusively via `WorkflowServices.ExecuteWorkflow()` through RPC. No HTTP endpoint triggers, no webhook listeners.

---

## Execution Order

| Phase | Description | Dependencies |
|---|---|---|
| **Phase 1** | Create `AppEndWorkflow` project + NuGet + references | None |
| **Phase 2** | `ElsaSetup.cs` with DI and middleware extension methods | Phase 1 |
| **Phase 3** | Wire into `AppEndHost` (csproj + Program.cs) | Phase 2 |
| **Phase 4** | 4 sample workflows (HelloWorld, ScheduledDbCheck, OrderApproval, DataPipeline) | Phase 2 |
| **Phase 5** | Build & verify everything works | Phase 3 + 4 |
| **Phase 6** | `WorkflowServices.cs` + 4 admin Vue.js components + 1 shared kartabl component | Phase 5 |
| **Phase 7** | Custom Activity Library — 48 activities in 14 categories (Notifications, DB, AppEnd, Human Tasks, Data/Docs, HTTP, FileSystem, Text, Security, Collections, Flow Control, Archive, Math, Cache) | Phase 5 |

Each phase requires explicit approval before proceeding.
