# Phase 6 — Vue.js Workflow Management UI

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## Goal
Build a management interface for workflows using the existing Vue.js + jQuery stack, communicating through the existing `rpcAEP` pattern.

## 6.1 — Backend Bridge: `AppEndWorkflow/WorkflowServices.cs`

Static methods callable via `rpcAEP`:

### Workflow Definitions (CRUD)
| Method | Parameters | Returns | Description |
|---|---|---|---|
| `GetWorkflowDefinitions` | `filter`, `page`, `pageSize` | Paged list of definitions | List all workflows with filtering |
| `GetWorkflowDefinition` | `definitionId` | Single definition detail | Get one workflow's full details |
| `CreateWorkflowDefinition` | `name`, `description`, activities JSON | Created definition | Create a new workflow |
| `UpdateWorkflowDefinition` | `definitionId`, updated fields | Updated definition | Edit existing workflow |
| `DeleteWorkflowDefinition` | `definitionId` | Success/fail | Delete a workflow |
| `PublishWorkflowDefinition` | `definitionId` | Published definition | Activate a workflow |
| `RetractWorkflowDefinition` | `definitionId` | Retracted definition | Deactivate a workflow |

### Workflow Instances (Monitoring & Execution)
| Method | Parameters | Returns | Description |
|---|---|---|---|
| `GetWorkflowInstances` | `filter`, `status`, `page`, `pageSize` | Paged list of instances | List all executions |
| `GetWorkflowInstance` | `instanceId` | Single instance detail | Get execution details |
| `ExecuteWorkflow` | `definitionId`, `inputParams` | Instance info | Manually trigger a workflow |
| `CancelWorkflowInstance` | `instanceId` | Success/fail | Cancel a running instance |

### Inbox / Kartabl (User-facing)
| Method | Parameters | Returns | Description |
|---|---|---|---|
| `GetMyWorkflowTasks` | `status`, `page`, `pageSize` | Paged list of pending tasks | Get tasks assigned to current user |
| `CompleteWorkflowTask` | `taskId`, `outcome`, `outputParams` | Success/fail | Complete (approve/reject/...) a pending task |

### Metadata & Logs
| Method | Parameters | Returns | Description |
|---|---|---|---|
| `GetActivityDescriptors` | — | List of activity types | All available activities |
| `GetWorkflowExecutionLog` | `instanceId` | List of log entries | Step-by-step execution log |

### Data Flow
```
Vue Component                    AppEnd RPC              Elsa SDK
─────────────                    ──────────              ────────

WorkflowDefinitions.vue          WorkflowServices        IWorkflowDefinitionStore
  │                                │                       │
  ├── rpcAEP("GetWorkflow         ├── GetWorkflow          ├── store.FindManyAsync()
  │   Definitions", {filter})     │   Definitions()        │
  │                                │                       │
  ├── rpcAEP("CreateWorkflow      ├── CreateWorkflow       ├── store.SaveAsync()
  │   Definition", {name,...})    │   Definition()         │
  │                                │                       │
  └── rpcAEP("ExecuteWorkflow",   └── ExecuteWorkflow()   └── dispatcher.DispatchAsync()
      {definitionId, inputs})
```

---

## 6.2 — Frontend: Vue.js Components

All components follow the existing pattern:
- Card layout with `card-header` / `card-body` / `card-footer`
- `rpcAEP` for server communication
- `_this = { cid: "", c: null, ... }` data pattern
- Bootstrap 5 classes + FontAwesome icons
- Pagination with `pageSize` / `pageNumber`

**Component placement:**

| Component | Path | Reason |
|---|---|---|
| `WorkflowDefinitions.vue` | `AppEndStudio/components/` | Admin-only: workflow CRUD |
| `WorkflowInstances.vue` | `AppEndStudio/components/` | Admin-only: execution monitoring |
| `WorkflowInstanceDetail.vue` | `AppEndStudio/components/` | Admin-only: instance detail |
| `WorkflowActivityBrowser.vue` | `AppEndStudio/components/` | Admin-only: activity reference |
| `WorkflowInbox.vue` | `a.SharedComponents/` | User-facing: kartabl / task inbox |

---

### Component 1: `WorkflowDefinitions.vue` — Workflow CRUD (Admin)

**Path:** `AppEndStudio/components/`

**Similar to:** `BaseCacheManagement.vue`

**Layout:**

| Area | Content |
|---|---|
| **Header** | `+ New Workflow` button, `Refresh` button, Search input |
| **Body** | Table of workflow definitions |
| **Footer** | Pagination (pageSize selector + page navigation + stats) |

**Table Columns:**

| # | Name | Description | Status | Version | Last Modified | Actions |
|---|---|---|---|---|---|---|
| 1 | Order Approval | Approves orders | 🟢 Published | v3 | 2025-01-15 | ▶️ 📝 ⬆️ 🗑️ |
| 2 | User Onboarding | New user flow | 🟡 Draft | v1 | 2025-01-14 | ▶️ 📝 ⬆️ 🗑️ |

**Row Actions:**
- ▶️ **Execute** — Opens modal for input parameters, then triggers `rpcAEP("ExecuteWorkflow", ...)`
- 📝 **Edit** — Opens inline/modal edit form
- ⬆️ **Publish / Retract** — Toggle workflow active state
- 🗑️ **Delete** — Delete with confirmation

**Create/Edit Form (Modal):**
- `Name` — Text input
- `Description` — Textarea
- `Activities` — JSON editor (simple textarea for activity definitions)
- `Variables` — Key-value editor for workflow variables

---

### Component 2: `WorkflowInstances.vue` — Execution Monitoring (Admin)

**Path:** `AppEndStudio/components/`

**Similar to:** `WorkersParalleled.vue`

**Layout:**

| Area | Content |
|---|---|
| **Header** | `Refresh` button, Status filter dropdown, Workflow name filter |
| **Body** | Table of workflow instances |
| **Footer** | Pagination |

**Table Columns:**

| # | Workflow | Instance ID | Status | Started | Completed | Duration | Actions |
|---|---|---|---|---|---|---|---|
| 1 | Order Approval | abc-123 | ✅ Completed | 14:30 | 14:31 | 42s | 👁️ |
| 2 | User Onboarding | def-456 | 🔄 Running | 14:35 | — | — | 👁️ ❌ |
| 3 | Order Approval | ghi-789 | ❌ Faulted | 14:20 | 14:20 | 1s | 👁️ |

**Status Filter Options:**
`All` | `Running` | `Completed` | `Faulted` | `Cancelled` | `Suspended`

**Row Actions:**
- 👁️ **View Details** — Navigate to `WorkflowInstanceDetail`
- ❌ **Cancel** — Cancel a running instance (only for Running/Suspended)

**Auto-refresh:** Every 10 seconds (same pattern as `WorkersParalleled.vue` `refreshEvery(10)`)

---

### Component 3: `WorkflowInstanceDetail.vue` — Single Execution Detail (Admin)

**Path:** `AppEndStudio/components/`

**Opens when:** User clicks 👁️ View Details on an instance

**Layout:**

| Area | Content |
|---|---|
| **Header** | Workflow Name + Instance ID + Status badge + Back button |
| **Body (top)** | Summary cards: Started, Completed, Duration, Status, Fault message |
| **Body (bottom)** | Execution Log table — step-by-step activity execution |

**Execution Log Table:**

| # | Activity Name | Type | Status | Started | Duration | Output |
|---|---|---|---|---|---|---|
| 1 | Receive Request | HttpEndpoint | ✅ Done | 14:30:01 | 5ms | 👁️ |
| 2 | Set OrderId | SetVariable | ✅ Done | 14:30:01 | 1ms | 👁️ |
| 3 | Validate Order | RunJavaScript | ❌ Faulted | 14:30:02 | 12ms | 👁️ |

👁️ on Output: Shows JSON output of each activity using existing `showJson()` utility.

---

### Component 4: `WorkflowActivityBrowser.vue` — Activity Reference (Admin)

**Path:** `AppEndStudio/components/`

**Goal:** Show all available Elsa activities to help users build workflows.

**Layout:**

| Area | Content |
|---|---|
| **Header** | Search input + Category filter dropdown |
| **Body** | Grid of activity cards (responsive `row-cols-1 row-cols-md-3`) |

**Each Activity Card:**
```
┌──────────────────────────────┐
│ 🌐 HTTP Endpoint             │
│ Category: HTTP               │
│ ─────────────────────────────│
│ Listens for incoming         │
│ HTTP requests.               │
│                              │
│ Inputs:  Path, Methods       │
│ Outputs: Request             │
└──────────────────────────────┘
```

**Data source:** `rpcAEP("GetActivityDescriptors", {}, callback)` — returns all registered activity types from Elsa.

---

### Component 5: `WorkflowInbox.vue` — Kartabl / Task Inbox (User-facing)

**Path:** `a.SharedComponents/` — shared across all apps, accessible to all authenticated users.

**Similar to:** `MySummary.vue` / `MyShortcuts.vue` (user-facing shared components)

**Goal:** Show pending workflow tasks assigned to the current user and allow them to take action (approve, reject, complete).

**Layout:**

| Area | Content |
|---|---|
| **Header** | `Refresh` button, Status filter (`Pending` / `Completed` / `All`) |
| **Body** | Table of assigned tasks |
| **Footer** | Pagination |

**Table Columns:**

| # | Workflow | Task | Assigned On | Due Date | Status | Actions |
|---|---|---|---|---|---|---|
| 1 | Order Approval | Review Order #1042 | 2025-01-15 10:30 | 2025-01-16 | ⏳ Pending | ✅ ❌ 👁️ |
| 2 | Leave Request | Approve Leave | 2025-01-15 09:00 | 2025-01-17 | ⏳ Pending | ✅ ❌ 👁️ |
| 3 | Order Approval | Review Order #1038 | 2025-01-14 14:00 | — | ✅ Completed | 👁️ |

**Row Actions:**
- ✅ **Approve / Complete** — `rpcAEP("CompleteWorkflowTask", { TaskId: "...", Outcome: "Approved" })`
- ❌ **Reject** — `rpcAEP("CompleteWorkflowTask", { TaskId: "...", Outcome: "Rejected" })`
- 👁️ **View Details** — Shows task context (workflow data, history) via `showJson()`

**Auto-refresh:** Every 15 seconds.

**User context:** Uses `shared.getLogedInUserContext()` to identify the current user and filter tasks accordingly.

---

## 6.3 — Styles

**Dedicated CSS file:** `a..lib/append/css/append-workflow.css`

> **Note:** Phase 7 (Custom Activity Library) should be implemented after Phase 6 is complete and verified.

Following the existing naming convention (`append-components.css`, `append-designer.css`, `append-forms.css`, etc.), all workflow-related styles go into a single dedicated file.

- **NOT** in `AppEndStudio/assets/custom.css`
- **NOT** in existing `append-components.css`
- **NEW** file: `a..lib/append/css/append-workflow.css`
- Use existing Bootstrap 5 utility classes wherever possible
- FontAwesome icons (already available)
- Status badges: Bootstrap badge classes (`bg-success`, `bg-warning`, `bg-danger`, `bg-info`)
