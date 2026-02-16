# RPC API Reference - Workflow System

**آخرین آپدیت:** 2025-01-16

---

## 📡 Workflow RPC Endpoints

### 1️⃣ GetWorkflowDefinitions
**مقصد:** لیست تمام workflow definitions  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("GetWorkflowDefinitions", {}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "definitions": [
    {
      "id": "hello-world",
      "name": "Hello World",
      "description": "A simple greeting workflow",
      "version": 1,
      "isPublished": true,
      "loadedAt": "2025-01-16T10:00:00Z"
    }
  ]
}
```

---

### 2️⃣ GetWorkflowDefinition
**مقصد:** دریافت جزئیات یک workflow  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("GetWorkflowDefinition", { 
    WorkflowId: "order-approval"
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "definition": {
    "id": "order-approval",
    "name": "Order Approval",
    "description": "Approve or reject customer orders",
    "version": 2,
    "isPublished": true,
    "loadedAt": "2025-01-16T10:00:00Z",
    "rawJson": "{...}"
  }
}
```

---

### 3️⃣ ExecuteWorkflow
**مقصد:** اجرای یک workflow  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("ExecuteWorkflow", { 
    WorkflowId: "order-approval",
    InputParams: {
        orderId: 12345,
        amount: 2500000,
        customerName: "احمد علی"
    }
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "instanceId": "abc123...",
  "status": "Running",
  "output": null,
  "executedAt": "2025-01-16T10:05:00Z"
}
```

---

### 4️⃣ GetWorkflowInstances ⭐ NEW
**مقصد:** لیست تمام workflow instances  
**وضعیت:** ✅ پیاده‌سازی شده

```javascript
rpcAEP("GetWorkflowInstances", { 
    Status: "Running",      // Optional: Running, Finished, Failed, Cancelled
    Filter: "order",        // Optional: filter by definitionId
    Page: 1,
    PageSize: 25
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "instances": [
    {
      "instanceId": "abc123def456",
      "definitionId": "order-approval",
      "definitionName": "Order Approval",
      "status": "Running",
      "startedAt": "2025-01-16T10:00:00Z",
      "finishedAt": null,
      "lastExecutedAt": "2025-01-16T10:05:00Z",
      "incidentCount": 0
    }
  ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 25,
  "totalPages": 2
}
```

---

### 5️⃣ GetWorkflowInstance
**مقصد:** جزئیات یک workflow instance  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("GetWorkflowInstance", { 
    InstanceId: "abc123def456"
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "instance": {
    "instanceId": "abc123def456",
    "definitionId": "order-approval",
    "status": "Suspended",
    "startedAt": "2025-01-16T10:00:00Z",
    "finishedAt": null,
    "lastExecutedAt": "2025-01-16T10:05:00Z",
    "incidentCount": 0
  }
}
```

---

### 6️⃣ CancelWorkflowInstance
**مقصد:** لغو یک workflow instance  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("CancelWorkflowInstance", { 
    InstanceId: "abc123def456"
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "message": "Workflow instance cancelled successfully",
  "instanceId": "abc123def456",
  "cancelledAt": "2025-01-16T10:10:00Z"
}
```

---

### 7️⃣ ReloadWorkflow
**مقصد:** بارگذاری مجدد یک workflow definition  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("ReloadWorkflow", { 
    WorkflowId: "order-approval"
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "message": "Workflow 'order-approval' reloaded successfully",
  "workflow": {
    "id": "order-approval",
    "name": "Order Approval",
    "version": 2,
    "isPublished": true
  },
  "timestamp": "2025-01-16T10:10:00Z"
}
```

---

### 8️⃣ ReloadAllWorkflows
**مقصد:** بارگذاری مجدد تمام workflows  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("ReloadAllWorkflows", {}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "message": "All workflows reloaded successfully",
  "count": 5,
  "timestamp": "2025-01-16T10:10:00Z"
}
```

---

### 9️⃣ GetWorkflowStats
**مقصد:** آمار کلی workflows  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("GetWorkflowStats", {}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "stats": {
    "totalDefinitions": 5,
    "publishedDefinitions": 4,
    "totalInstances": 42,
    "runningInstances": 8,
    "completedInstances": 30,
    "failedInstances": 2,
    "totalTasks": 15,
    "pendingTasks": 3
  }
}
```

---

### 🔟 GetWorkflowExecutionLog
**مقصد:** لاگ اجرای workflow  
**وضعیت:** ✅ موجود

```javascript
rpcAEP("GetWorkflowExecutionLog", { 
    InstanceId: "abc123def456"
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "logs": [
    {
      "timestamp": "2025-01-16T10:00:00Z",
      "eventType": "WorkflowStarted",
      "description": "Workflow instance started"
    },
    {
      "timestamp": "2025-01-16T10:01:00Z",
      "eventType": "ActivityExecuted",
      "description": "Activity 'SendEmail' executed"
    }
  ]
}
```

---

## 🎯 Task Management RPC Endpoints (NEW)

### 1️⃣ GetMyWorkflowTasks ⭐ NEW
**مقصد:** دریافت تسک‌های تخصیص‌یافته به کاربر فعلی  
**وضعیت:** ✅ پیاده‌سازی شده  
**تاریخ اضافه:** 2025-01-16

```javascript
rpcAEP("GetMyWorkflowTasks", { 
    Status: "Pending",      // Optional: Pending, Completed, Cancelled
    Page: 1,
    PageSize: 25
}, (result) => {
    console.log(result);
});
```

**Response:**
```json
{
  "success": true,
  "tasks": [
    {
      "taskId": "task-guid-here",
      "workflowInstanceId": "instance-guid-here",
      "workflowDefinitionId": "order-approval",
      "title": "تایید سفارش #12345",
      "description": "سفارشی به مبلغ ۲٫۵ میلیون تومان",
      "priority": "High",
      "status": "Pending",
      "assignedTo": "admin",
      "dueDate": "2025-01-19T00:00:00Z",
      "createdAt": "2025-01-16T10:00:00Z",
      "contextData": "{\"orderId\": 12345, \"amount\": 2500000}"
    }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 25,
  "totalPages": 1
}
```

**خطاها:**
```json
{
  "success": false,
  "error": "Task not found or database error"
}
```

---

### 2️⃣ CompleteWorkflowTask ⭐ NEW
**مقصد:** تکمیل یک تسک و resume workflow  
**وضعیت:** ✅ پیاده‌سازی شده  
**تاریخ اضافه:** 2025-01-16

```javascript
rpcAEP("CompleteWorkflowTask", { 
    TaskId: "task-guid-here",
    Outcome: "Approve",           // Approve, Reject, Escalate, etc.
    OutputParams: {
        comment: "تایید شد - خوب است",
        approvalDate: new Date().toISOString()
    }
}, (result) => {
    console.log(result);
});
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Task completed successfully",
  "taskId": "task-guid-here",
  "outcome": "Approve",
  "completedAt": "2025-01-16T10:05:00Z",
  "completedBy": "admin",
  "bookmarkResumed": true
}
```

**Response (Already Completed):**
```json
{
  "success": false,
  "error": "Task not found or already completed",
  "taskId": "task-guid-here"
}
```

---

## 📊 مقایسه قبل و بعد

| API | قبل | بعد |
|-----|------|------|
| GetWorkflowInstances | Mock | Real ✅ |
| GetWorkflowInstance | Mock | Real ✅ |
| ExecuteWorkflow | Real | Real ✅ |
| GetMyWorkflowTasks | Mock | Real ✅ |
| CompleteWorkflowTask | Stub | Real ✅ |

---

## 🔧 استفاده در Components

### WorkflowInstances.vue
```javascript
// قبل: Mock data
refreshInstances() { 
    this.instances = [...hardcoded...] 
}

// بعد: Real API
refreshInstances() {
    rpcAEP("GetWorkflowInstances", {...}, (result) => {
        this.instances = result.instances
    })
}
```

### WorkflowInbox.vue
```javascript
// قبل: Mock data
refreshTasks() { 
    this.tasks = [...hardcoded...] 
}

// بعد: Real API
refreshTasks() {
    rpcAEP("GetMyWorkflowTasks", {...}, (result) => {
        this.tasks = result.tasks.map(...)
    })
}

// قبل: Stub
async completeTask(outcome) { /* ... */ }

// بعد: Real implementation
async completeTask(outcome) {
    rpcAEP("CompleteWorkflowTask", {
        TaskId: this.selectedTask.TaskId,
        Outcome: outcome,
        OutputParams: { comment: this.taskComment }
    }, ...)
}
```

---

## ⚠️ نکات مهم

1. **User Context**: تمام endpoints اطلاعات کاربر از Actor می‌گیرند
2. **Pagination**: صفحات از 1 شروع می‌شوند
3. **Filtering**: Status filters حساس به بزرگ و کوچک هستند
4. **Error Handling**: همه responses دارای success flag هستند
5. **Dates**: تمام تاریخ‌ها ISO 8601 format هستند

---

## 🚀 نمونه Complete Workflow

```javascript
// 1. دریافت تسک‌های pending
rpcAEP("GetMyWorkflowTasks", { Status: "Pending" }, (tasks) => {
    
    // 2. انتخاب یک تسک
    const selectedTask = tasks.tasks[0];
    console.log("Approving:", selectedTask.title);
    
    // 3. تکمیل تسک
    rpcAEP("CompleteWorkflowTask", {
        TaskId: selectedTask.taskId,
        Outcome: "Approve",
        OutputParams: { comment: "تایید شد" }
    }, (result) => {
        if (result.success) {
            console.log("✅ Task completed!");
            console.log("Workflow resumed:", result.bookmarkResumed);
            
            // 4. بهبروزرسانی لیست تسک‌ها
            rpcAEP("GetMyWorkflowTasks", {}, refresh);
        }
    });
});
```

---

**Document Generated:** 2025-01-16  
**API Version:** 1.0  
**Status:** Production Ready
