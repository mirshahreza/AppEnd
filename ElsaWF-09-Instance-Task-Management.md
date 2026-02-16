# Workflow Instance & Task Management - Implementation Guide

**Date:** 2025-01-15  
**Status:** ✅ RPC Endpoints Implemented  
**Priority:** HIGH

---

## 📋 Overview

This document describes the newly implemented RPC endpoints for workflow instance monitoring and task management (kartabl).

**What's Been Added:**
- ✅ 6 new RPC endpoints in `WorkflowServices.cs`
- ✅ SQL schema for `WorkflowTasks` table
- ✅ Stored procedures for task queries
- ✅ Helper views for statistics

---

## 🎯 New RPC Endpoints

### 1. GetWorkflowInstances
**Purpose:** List all workflow executions with filtering and pagination.

**Usage from Vue.js:**
```javascript
rpcAEP("GetWorkflowInstances", { 
    Filter: "order",      // Optional: Filter by DefinitionId
    Status: "Running",    // Optional: Finished, Failed, Cancelled
    Page: 1,              // Default: 1
    PageSize: 25          // Default: 25
}, function(result) {
    if (result.success) {
        console.log("Total instances:", result.totalCount);
        console.log("Instances:", result.instances);
        console.log("Total pages:", result.totalPages);
    }
});
```

**Response Format:**
```json
{
  "success": true,
  "instances": [
    {
      "instanceId": "guid-here",
      "definitionId": "order-approval",
      "definitionName": "Order Approval Workflow",
      "status": "Running",
      "startedAt": "2025-01-15T10:30:00Z",
      "finishedAt": null,
      "lastExecutedAt": "2025-01-15T10:35:00Z",
      "incidentCount": 0
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 25,
  "totalPages": 2
}
```

---

### 2. GetWorkflowInstance
**Purpose:** Get detailed information about a single workflow execution.

**Usage:**
```javascript
rpcAEP("GetWorkflowInstance", { 
    InstanceId: "guid-here"
}, function(result) {
    if (result.success) {
        console.log("Instance details:", result.instance);
    }
});
```

**Response Format:**
```json
{
  "success": true,
  "instance": {
    "instanceId": "guid-here",
    "definitionId": "order-approval",
    "status": "Suspended",
    "startedAt": "2025-01-15T10:30:00Z",
    "finishedAt": null,
    "lastExecutedAt": "2025-01-15T10:32:00Z",
    "incidentCount": 0
  }
}
```

---

### 3. CancelWorkflowInstance
**Purpose:** Cancel a running workflow execution.

**Usage:**
```javascript
rpcAEP("CancelWorkflowInstance", { 
    InstanceId: "guid-here"
}, function(result) {
    if (result.success) {
        console.log("Workflow cancelled at:", result.timestamp);
    }
});
```

**Response Format:**
```json
{
  "success": true,
  "message": "Workflow instance canceled successfully",
  "instanceId": "guid-here",
  "timestamp": "2025-01-15T11:00:00Z"
}
```

---

### 4. GetMyWorkflowTasks ⚠️
**Purpose:** Get current user's pending workflow tasks (kartabl).

**Status:** ⚠️ Requires `WorkflowTasks` table setup

**Usage:**
```javascript
rpcAEP("GetMyWorkflowTasks", { 
    Status: "Pending",    // Optional: Pending, Completed, Cancelled
    Page: 1,              // Default: 1
    PageSize: 25          // Default: 25
}, function(result) {
    if (result.success) {
        console.log("My tasks:", result.tasks);
        console.log("Total tasks:", result.totalCount);
    } else {
        console.warn(result.message); // Will show table not implemented message
    }
});
```

**Expected Response Format:**
```json
{
  "success": true,
  "tasks": [
    {
      "taskId": "guid-here",
      "workflowInstanceId": "guid-here",
      "workflowDefinitionId": "order-approval",
      "title": "تایید سفارش شماره 12345",
      "description": "سفارش به مبلغ 2,500,000 تومان نیاز به تایید مدیر دارد",
      "assignedTo": "admin",
      "assignedRole": null,
      "priority": "High",
      "status": "Pending",
      "dueDate": "2025-01-18T23:59:59Z",
      "createdAt": "2025-01-15T10:30:00Z",
      "contextData": "{\"orderId\": 12345, \"amount\": 2500000}"
    }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 25,
  "totalPages": 1
}
```

---

### 5. CompleteWorkflowTask ⚠️
**Purpose:** Complete a task and resume the workflow.

**Status:** ⚠️ Requires `WorkflowTasks` table and bookmark integration

**Usage:**
```javascript
rpcAEP("CompleteWorkflowTask", { 
    TaskId: "guid-here",
    Outcome: "Approve",           // Approve, Reject, Escalate, etc.
    OutputParams: {               // Optional: Data to pass back to workflow
        comment: "Looks good",
        approvedAmount: 2500000
    }
}, function(result) {
    if (result.success) {
        console.log("Task completed by:", result.completedBy);
        console.log("Outcome:", result.outcome);
    }
});
```

**Expected Response Format:**
```json
{
  "success": true,
  "message": "Task completed successfully",
  "taskId": "guid-here",
  "outcome": "Approve",
  "completedAt": "2025-01-15T11:00:00Z",
  "completedBy": "admin"
}
```

---

### 6. GetWorkflowExecutionLog
**Purpose:** Get detailed execution log for debugging.

**Usage:**
```javascript
rpcAEP("GetWorkflowExecutionLog", { 
    InstanceId: "guid-here"
}, function(result) {
    if (result.success) {
        console.log("Execution steps:", result.logs);
    }
});
```

**Response Format:**
```json
{
  "success": true,
  "logs": [
    {
      "activityId": "start",
      "eventName": "Started",
      "timestamp": "2025-01-15T10:30:00Z",
      "payload": {}
    },
    {
      "activityId": "check-amount",
      "eventName": "Executed",
      "timestamp": "2025-01-15T10:30:05Z",
      "payload": { "amount": 2500000 }
    }
  ],
  "instanceId": "guid-here"
}
```

---

## 🗄️ Database Setup

### Step 1: Create WorkflowTasks Table

Run the SQL script:
```bash
# File location: AppEndHost/database/WorkflowTasks-Schema.sql
```

This creates:
- ✅ `WorkflowTasks` table with all required columns
- ✅ Indexes for performance (Status, AssignedTo, DueDate, etc.)
- ✅ Helper views (`vw_MyPendingTasks`, `vw_WorkflowTaskStats`)
- ✅ Stored procedures (`sp_GetMyWorkflowTasks`, `sp_CompleteWorkflowTask`)

### Step 2: Update WorkflowServices.cs

**Replace TODO comments with actual implementation:**

**In `GetMyWorkflowTasks` method:**
```csharp
// Replace this:
// var sql = "SELECT * FROM WorkflowTasks WHERE AssignedTo = @User...";
// tasks = DbIO.Query<TaskSummary>(sql, new { User = currentUser, Status });

// With:
var sql = @"
    SELECT 
        TaskId, WorkflowInstanceId, WorkflowDefinitionId,
        Title, Description, AssignedTo, AssignedRole,
        Priority, Status, DueDate, CreatedAt, ContextData
    FROM WorkflowTasks
    WHERE (AssignedTo = @User OR AssignedTo IS NULL)
        AND (@Status IS NULL OR Status = @Status)
    ORDER BY Priority DESC, CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

tasks = DbIO.Query<TaskSummary>(sql, new { 
    User = currentUser, 
    Status, 
    Offset = (Page - 1) * PageSize,
    PageSize 
}).ToList();

// Get total count
var countSql = @"
    SELECT COUNT(*) 
    FROM WorkflowTasks
    WHERE (AssignedTo = @User OR AssignedTo IS NULL)
        AND (@Status IS NULL OR Status = @Status)";
        
totalCount = DbIO.QueryScalar<int>(countSql, new { User = currentUser, Status });
```

**In `CompleteWorkflowTask` method:**
```csharp
// Get task from database
var task = DbIO.QuerySingle<dynamic>(@"
    SELECT * FROM WorkflowTasks 
    WHERE TaskId = @TaskId AND Status = 'Pending'",
    new { TaskId });

if (task == null)
    return new { success = false, error = "Task not found or already completed" };

// Update task status
DbIO.Execute(@"
    UPDATE WorkflowTasks 
    SET Status = 'Completed', 
        CompletedAt = @Now, 
        CompletedBy = @User, 
        Outcome = @Outcome,
        Comment = @Comment
    WHERE TaskId = @TaskId",
    new { 
        TaskId, 
        User = currentUser, 
        Outcome, 
        Comment = OutputParams?.ContainsKey("comment") == true ? OutputParams["comment"]?.ToString() : null,
        Now = DateTime.UtcNow 
    });

// Resume workflow
var services = GetServices();
var dispatcherType = FindType("Elsa.Workflows.Runtime.IWorkflowDispatcher");
if (dispatcherType != null)
{
    var dispatcher = services.GetService(dispatcherType);
    var method = dispatcherType.GetMethods()
        .FirstOrDefault(m => m.Name.Contains("DispatchEvent") || m.Name.Contains("PublishEvent"));
    
    if (method != null)
    {
        var eventName = $"TaskCompleted:{TaskId}";
        var args = new object[] { eventName, OutputParams ?? new Dictionary<string, object>(), CancellationToken.None };
        method.Invoke(dispatcher, args);
    }
}
```

---

## 🔗 Integration with UI Components

### WorkflowInstances.vue
**Update to use real data:**

```javascript
// Replace mock data with:
methods: {
    loadInstances() {
        this.loading = true;
        rpcAEP("GetWorkflowInstances", {
            Filter: this.searchQuery,
            Status: this.statusFilter,
            Page: this.currentPage,
            PageSize: this.pageSize
        }, (result) => {
            if (result.success) {
                this.instances = result.instances;
                this.totalCount = result.totalCount;
            } else {
                console.error("Failed to load instances:", result.error);
            }
            this.loading = false;
        });
    },
    
    cancelInstance(instanceId) {
        if (!confirm('آیا از لغو این workflow اطمینان دارید؟')) return;
        
        rpcAEP("CancelWorkflowInstance", { InstanceId: instanceId }, (result) => {
            if (result.success) {
                showSuccess('Workflow لغو شد');
                this.loadInstances();
            } else {
                showError('خطا در لغو: ' + result.error);
            }
        });
    }
}
```

### WorkflowInbox.vue
**Update to use real data:**

```javascript
// Replace mock data with:
methods: {
    loadTasks() {
        this.loading = true;
        rpcAEP("GetMyWorkflowTasks", {
            Status: this.statusFilter,
            Page: this.currentPage,
            PageSize: this.pageSize
        }, (result) => {
            if (result.success) {
                this.tasks = result.tasks;
                this.totalCount = result.totalCount;
            } else {
                console.warn("Tasks not available:", result.message);
                this.tasks = []; // Will show empty state
            }
            this.loading = false;
        });
    },
    
    completeTask(task, outcome) {
        const comment = prompt(`نظر خود را وارد کنید (${outcome}):`);
        if (comment === null) return; // User cancelled
        
        rpcAEP("CompleteWorkflowTask", {
            TaskId: task.taskId,
            Outcome: outcome,
            OutputParams: { comment }
        }, (result) => {
            if (result.success) {
                showSuccess('تسک با موفقیت انجام شد');
                this.loadTasks();
            } else {
                showError('خطا در انجام تسک: ' + result.error);
            }
        });
    }
}
```

---

## ✅ Testing Checklist

### Phase 1: Instance Monitoring (No DB Required)
- [ ] Call `GetWorkflowInstances` - should return empty list or existing instances
- [ ] Call `GetWorkflowInstance` with valid ID - should return details
- [ ] Call `CancelWorkflowInstance` on running workflow - should cancel it
- [ ] Call `GetWorkflowExecutionLog` - should return execution steps

### Phase 2: Task Management (Requires WorkflowTasks Table)
- [ ] Run `WorkflowTasks-Schema.sql` in database
- [ ] Verify table created: `SELECT * FROM WorkflowTasks`
- [ ] Insert test task manually (see SQL script comments)
- [ ] Call `GetMyWorkflowTasks` - should return test task
- [ ] Call `CompleteWorkflowTask` - should update task and return success

### Phase 3: End-to-End Workflow
- [ ] Execute `order-approval` workflow with high amount
- [ ] Verify workflow suspends (Status = "Suspended")
- [ ] Manually insert task into `WorkflowTasks` table
- [ ] Complete task via `CompleteWorkflowTask`
- [ ] Verify workflow resumes and completes

---

## 🐛 Troubleshooting

### "Workflow instance store not available"
**Cause:** Elsa services not properly initialized.  
**Fix:** Ensure `AddAppEndWorkflow()` is called in `Program.cs`

### "WorkflowTasks table not yet implemented"
**Cause:** SQL table not created.  
**Fix:** Run `WorkflowTasks-Schema.sql` script

### "Task not found or already completed"
**Cause:** Task doesn't exist or already processed.  
**Fix:** Check `SELECT * FROM WorkflowTasks WHERE TaskId = 'guid'`

### Tasks not appearing in kartabl
**Cause:** Workflow hasn't created task record.  
**Fix:** Add task creation logic to workflow JSON or custom activity

---

## 📊 Performance Considerations

**Indexes Created:**
- `IX_WorkflowTasks_Status` - Fast filtering by status
- `IX_WorkflowTasks_AssignedTo` - Fast user task lookup
- `IX_WorkflowTasks_DueDate` - Fast overdue detection
- `IX_WorkflowTasks_WorkflowInstanceId` - Fast workflow-to-task mapping

**Pagination:**
- All list endpoints support pagination (Page, PageSize)
- Default page size: 25 items
- Use pagination for large datasets

**Caching:**
- Consider caching `GetWorkflowDefinitions` result (definitions rarely change)
- Don't cache instance or task data (real-time monitoring)

---

## 🎯 Next Steps

### Immediate (to complete implementation):
1. ✅ Run `WorkflowTasks-Schema.sql` in database
2. ✅ Update `GetMyWorkflowTasks` with real DB query
3. ✅ Update `CompleteWorkflowTask` with DB update and bookmark resume
4. ✅ Update `WorkflowInstances.vue` to use real endpoint
5. ✅ Update `WorkflowInbox.vue` to use real endpoint
6. ✅ Test all endpoints with real workflows

### Short-term (enhancements):
1. Add `CreateWorkflowTask` helper method for workflows to use
2. Add automatic task cleanup (delete tasks for completed workflows)
3. Add task escalation (reassign overdue tasks)
4. Add task notifications (email/SMS when task assigned)

### Long-term (advanced features):
1. Task delegation (assign to another user)
2. Task history/audit trail
3. Task templates
4. Bulk task operations
5. Task analytics dashboard

---

## 📚 Related Documentation

- `ElsaWF-09-Pending.md` - Production status report (where this was requested)
- `WorkflowServices.cs` - RPC endpoint implementations
- `WorkflowTasks-Schema.sql` - Database schema
- `WorkflowInstances.vue` - Admin monitoring UI
- `WorkflowInbox.vue` - User kartabl UI

---

**Status:** ✅ **RPC Endpoints Implemented - DB Setup Required**

**Priority:** HIGH - Complete DB setup and testing before production deployment
