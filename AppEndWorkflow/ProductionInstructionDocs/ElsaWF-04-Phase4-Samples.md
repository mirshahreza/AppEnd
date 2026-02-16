# Phase 4 — Sample Workflows

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## Goal
Create multiple sample workflows to verify the integration end-to-end and serve as learning references for developers.

All sample workflows are code-based, auto-discovered by Elsa on startup, and triggered exclusively via `rpcAEP` → `WorkflowServices` → Elsa SDK.

> **No HTTP endpoints.** None of these workflows have URL routes — they are all triggered
> exclusively through the RPC bridge.

---

## 4.1 — HelloWorld Workflow (Basic)

**File:** `AppEndWorkflow/Workflows/HelloWorldWorkflow.cs`
**Definition ID:** `hello-world`

**Purpose:** Simplest possible workflow — verifies Elsa is running and can execute.

**Steps:**
1. Sets a variable `Greeting` = `"Hello from Elsa!"`
2. Runs a JavaScript activity that builds a result object with `message` + `timestamp`
3. Completes and returns the result

**Test:**
```javascript
rpcAEP("ExecuteWorkflow", { DefinitionId: "hello-world", InputParams: {} }, function(res) {
    console.log(res);
});
```

**Teaches:** Basic workflow structure, SetVariable, RunJavaScript, returning output.

---

## 4.2 — Timer-Based DB Check Workflow (Scheduled)

**File:** `AppEndWorkflow/Workflows/ScheduledDbCheckWorkflow.cs`
**Definition ID:** `scheduled-db-check`

**Purpose:** Demonstrates timer/cron-triggered workflow that queries the database on a schedule and logs results.

**Steps:**
1. **Timer trigger** — Fires every 5 minutes (Cron: `*/5 * * * *`)
2. **RunJavaScript** — Executes a SQL query via `DbIO` to count pending orders (or any domain-relevant table)
3. **If** (condition) — Checks if count exceeds a threshold
4. **SetVariable** — Stores alert message with count + timestamp
5. **RunJavaScript** — Logs the result (writes to `AppEndLog`)

**Teaches:** Timer/Cron triggers, database interaction from workflow, conditional branching (If activity), logging patterns.

**Note:** This workflow starts automatically when published — no manual RPC trigger needed. Can also be triggered manually:
```javascript
rpcAEP("ExecuteWorkflow", { DefinitionId: "scheduled-db-check", InputParams: {} }, function(res) {
    console.log(res);
});
```

---

## 4.3 — Order Approval Workflow (Human Task / Kartabl)

**File:** `AppEndWorkflow/Workflows/OrderApprovalWorkflow.cs`
**Definition ID:** `order-approval`

**Purpose:** Demonstrates a multi-step approval workflow with human interaction — the core use case for kartabl (inbox).

**Steps:**
1. **SetVariable** — Receives `OrderId`, `OrderAmount`, `RequestedBy` from input parameters
2. **If** (condition) — If `OrderAmount > 1000000`, go to manager approval; otherwise auto-approve
3. **Fork** (for high-value orders):
   - **Branch A — Manager Approval:**
     1. **CreateTask** — Creates a task assigned to `Manager` role with order details
     2. **WaitForTask** — Suspends workflow, waits for manager to approve/reject via kartabl
     3. **If** — Manager approved → continue; rejected → go to rejection branch
   - **Branch B — Auto-Approved:**
     1. **SetVariable** — Sets `ApprovalStatus = "AutoApproved"`
4. **RunJavaScript** — Updates order status in database
5. **SetVariable** — Builds final result with `OrderId`, `Status`, `ApprovedBy`, `Timestamp`
6. **Complete** — Returns result

**Test (trigger new order approval):**
```javascript
rpcAEP("ExecuteWorkflow", {
    DefinitionId: "order-approval",
    InputParams: JSON.stringify({ OrderId: "ORD-1042", OrderAmount: 2500000, RequestedBy: "user1" })
}, function(res) {
    console.log(res); // Instance is now suspended, waiting for manager approval
});
```

**Test (manager completes task via kartabl):**
```javascript
rpcAEP("CompleteWorkflowTask", {
    TaskId: "task-id-here",
    Outcome: "Approved",
    OutputParams: JSON.stringify({ Comment: "Approved by manager" })
}, function(res) {
    console.log(res); // Workflow resumes and completes
});
```

**Teaches:** Input parameters, conditional branching (If), Fork/Join, human task creation, workflow suspension/resumption, kartabl integration, database updates from workflow.

---

## 4.4 — Multi-Step Data Pipeline Workflow (Sequential + Error Handling)

**File:** `AppEndWorkflow/Workflows/DataPipelineWorkflow.cs`
**Definition ID:** `data-pipeline`

**Purpose:** Demonstrates a sequential data processing pipeline with error handling and retry — common pattern for batch operations.

**Steps:**
1. **SetVariable** — Receives `SourceTable`, `TargetTable`, `BatchSize` from input
2. **RunJavaScript** — Step 1: Validates source table exists and has data, returns record count
3. **If** — No records → complete with "nothing to process" message
4. **RunJavaScript** — Step 2: Reads batch of records from source table
5. **ForEach** — Step 3: Iterates over each record:
   - **Try/Catch:**
     - **Try:** RunJavaScript — Transform and insert record into target table
     - **Catch:** SetVariable — Log failed record ID + error message, continue
6. **RunJavaScript** — Step 4: Build summary report (total, success, failed, duration)
7. **Complete** — Returns summary report

**Test:**
```javascript
rpcAEP("ExecuteWorkflow", {
    DefinitionId: "data-pipeline",
    InputParams: JSON.stringify({ SourceTable: "TempOrders", TargetTable: "Orders", BatchSize: 100 })
}, function(res) {
    console.log(res); // { Total: 85, Success: 82, Failed: 3, Duration: "12s" }
});
```

**Teaches:** ForEach loops, Try/Catch error handling, sequential multi-step processing, batch operations, building summary reports.

---

## Sample Workflows Summary

| # | Workflow | Definition ID | Trigger | Key Concepts |
|---|---|---|---|---|
| 1 | HelloWorld | `hello-world` | Manual (RPC) | Basics, SetVariable, JavaScript |
| 2 | Scheduled DB Check | `scheduled-db-check` | Timer (Cron) | Scheduled tasks, DB query, conditional |
| 3 | Order Approval | `order-approval` | Manual (RPC) | Human tasks, kartabl, Fork, suspension |
| 4 | Data Pipeline | `data-pipeline` | Manual (RPC) | ForEach, Try/Catch, batch processing |
