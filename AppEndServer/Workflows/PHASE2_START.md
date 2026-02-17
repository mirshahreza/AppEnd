# 🚀 Phase 2: Integration - شروع

**وضعیت**: ✅ **شروع شد**  
**Build**: ✅ **SUCCESS**  
**هدف**: اتصال Elsa به AppEnd Infrastructure

---

## 📋 Phase 2 Scope

### 1️⃣ **Scheduler Integration** (جدول 1)
**هدف**: Workflows را از AppEnd's Scheduler اجرا کنید

**باید انجام شود**:
- [ ] AppEnd Scheduler hooks
- [ ] Workflow trigger activities
- [ ] Cron expression support
- [ ] Scheduled workflow execution

**مثال**:
```csharp
// AppEnd scheduler یک workflow اجرا می‌کند
SchedulerService.RegisterWorkflowTask("approval-process", "*/5 * * * *");
```

---

### 2️⃣ **Event System Integration** (جدول 2)
**هدف**: AppEnd events با Elsa workflows رابطه برقرار کنند

**باید انجام شود**:
- [ ] Elsa event listeners
- [ ] AppEnd event publishers
- [ ] Event mapping
- [ ] Workflow resumption on events

**مثال**:
```csharp
// وقتی یک AppEnd event اتفاق می‌افتد، workflow resume شود
AppEndEventBus.On("DocumentApproved", () => 
{
    workflows.ResumeWorkflowAsync(instanceId);
});
```

---

### 3️⃣ **RPC Endpoints** (جدول 3)
**هدف**: RPC API برای Workflow Management

**باید انجام شود**:
- [ ] ExecuteWorkflow RPC
- [ ] GetWorkflowInstance RPC
- [ ] ListWorkflows RPC
- [ ] ResumeWorkflow RPC
- [ ] SuspendWorkflow RPC

**مثال**:
```csharp
// RpcNet endpoint
public class WorkflowProxy
{
    public async Task<string> ExecuteWorkflow(string definitionId, object input)
    {
        return await _workflows.ExecuteWorkflowAsync(definitionId, ...);
    }
}
```

---

### 4️⃣ **Workflow Execution** (جدول 4)
**هدف**: واقعی Workflow Execution

**باید انجام شود**:
- [ ] Execute workflow logic implementation
- [ ] Resume workflow logic
- [ ] Suspend workflow logic
- [ ] Cancel workflow logic
- [ ] Error handling
- [ ] Execution logging

**مثال**:
```csharp
// Service method اجرا می‌شود
public async Task<string> ExecuteWorkflowAsync(string definitionId, ...)
{
    var instance = await _workflowRuntime.StartWorkflowAsync(...);
    return instance.Id;
}
```

---

## 🎯 Phase 2 Tasks - Priority Order

| # | Task | Priority | Time |
|---|------|----------|------|
| 1 | Create Scheduler Integration | 🔴 High | 4h |
| 2 | Create Event System Hooks | 🔴 High | 3h |
| 3 | Create RPC Endpoints | 🟠 Medium | 5h |
| 4 | Implement Workflow Execution | 🔴 High | 6h |
| 5 | Error Handling & Logging | 🟠 Medium | 2h |
| 6 | Testing & Verification | 🔴 High | 4h |
| **Total** | | | **24h** |

---

## 📁 Files to Create/Modify

```
AppEndServer/Workflows/
├── Phase2/
│   ├── SchedulerIntegration.cs
│   ├── EventSystemHooks.cs
│   ├── RpcEndpoints.cs
│   └── ExecutionEngine.cs
├── Updated/
│   ├── WorkflowService.cs (implement actual logic)
│   ├── WorkflowDefinitionService.cs (implement actual logic)
│   └── WorkflowInstanceService.cs (implement actual logic)
└── Documentation/
    ├── PHASE2_SCHEDULER_INTEGRATION.md
    ├── PHASE2_EVENT_HOOKS.md
    ├── PHASE2_RPC_ENDPOINTS.md
    └── PHASE2_EXECUTION_ENGINE.md
```

---

## 🔄 Phase 2 Flow

```
AppEnd Events
     ↓
AppEnd Scheduler
     ↓
[Phase 2] Event/Scheduler Integration
     ↓
Elsa Workflow Execution
     ↓
Elsa Activities
     ↓
[Phase 3] Custom Activities
     ↓
AppEnd Operations
     ↓
RPC Responses
```

---

## ✅ Prerequisites for Phase 2

- [x] Phase 1 Foundation complete
- [x] Pre-Phase 2 checklist complete
- [x] Elsa configuration in appsettings.json
- [x] Database ready (ElsaWorkflows)
- [x] Build successful

---

## 🚀 شروع کار؟

**مرحله بعدی**:
1. Scheduler Integration را شروع کنیم
2. AppEnd's SchedulerService را بررسی کنیم
3. Workflow trigger logic را ایجاد کنیم

**آماده برای شروع؟** 💪

---

**Status**: ✅ READY FOR PHASE 2  
**Next**: Scheduler Integration Implementation
