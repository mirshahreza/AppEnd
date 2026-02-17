# Phase 2: Integration - Complete ✅

**Status**: All 4 parts implemented and compiling successfully

---

## Overview

Phase 2 integrates Elsa 3.0 workflows with AppEnd's existing infrastructure:
- **Scheduler Integration**: Register workflows as scheduled tasks via AppEnd SchedulerService
- **Event System Integration**: Publish/subscribe workflow events and coordinate with AppEnd operations
- **RPC Endpoints**: Expose workflow operations through AppEnd's RpcNet framework
- **Execution Engine**: Core workflow execution logic with TODO markers for Elsa implementation

---

## Part 1: Scheduler Integration ✅

**File**: `AppEndServer/Workflows/Phase2/WorkflowSchedulerIntegration.cs`

### Classes

#### `WorkflowSchedulerIntegration`
Manages registration of workflows as scheduled tasks in AppEnd's SchedulerService.

**Key Methods**:
- `RegisterWorkflowAsScheduledTaskAsync()` - Register workflow as cron-based task
- `UnregisterWorkflowTaskAsync()` - Remove scheduled workflow task
- `PauseWorkflowTaskAsync()` - Pause scheduled execution
- `ResumeWorkflowTaskAsync()` - Resume scheduled execution
- `GetWorkflowDefinitionForTask()` - Get workflow ID for scheduled task
- `GetAllWorkflowTasks()` - List all workflow scheduled tasks

**Features**:
- Serializes workflow parameters to JSON for ScheduledTask.MethodParameters
- Integrates with AppEnd's SchedulerService background task runner
- Maintains mapping of task IDs to workflow definition IDs
- Comprehensive logging for all operations

#### `WorkflowScheduledExecutor` (Static Helper)
Static helper class invoked by AppEnd scheduler to execute workflows at scheduled times.

**Key Methods**:
- `Initialize(IWorkflowService, ILoggerFactory)` - Setup DI services
- `ExecuteWorkflowAsync(string workflowDefinitionId, Dictionary<string, object>? input)` - Execute workflow

**Implementation Notes**:
- Uses ILoggerFactory to avoid C# generic type constraints on static classes
- Called by scheduler via ScheduledTask.MethodFullName
- Handles Elsa workflow execution (TODO placeholder)

**Example Usage**:
```csharp
var integration = serviceProvider.GetRequiredService<WorkflowSchedulerIntegration>();

// Register workflow as daily task at 2 AM
await integration.RegisterWorkflowAsScheduledTaskAsync(
    workflowDefinitionId: "approval-workflow-001",
    taskId: "daily-approval-check",
    cronExpression: "0 2 * * *",  // 2 AM daily
    enabled: true,
    input: new Dictionary<string, object> { { "priority", "high" } }
);

// Later, pause the task
await integration.PauseWorkflowTaskAsync("daily-approval-check");

// Resume the task
await integration.ResumeWorkflowTaskAsync("daily-approval-check");

// Unregister the task
await integration.UnregisterWorkflowTaskAsync("daily-approval-check");
```

---

## Part 2: Event System Integration ✅

**File**: `AppEndServer/Workflows/Phase2/WorkflowEventSystemIntegration.cs`

### Classes

#### `WorkflowEventSystemIntegration`
Implements pub/sub event system for workflow events and AppEnd event coordination.

**Key Methods**:
- `SubscribeToWorkflowEvent(string eventName, WorkflowEventHandler handler)` - Subscribe to workflow events
- `UnsubscribeFromWorkflowEvent(string eventName, WorkflowEventHandler handler)` - Unsubscribe from events
- `PublishWorkflowEventAsync(WorkflowEvent @event)` - Publish workflow event to all subscribers
- `SubscribeToAppEndEvent(string appEndEventName, string workflowInstanceId)` - React to AppEnd events
- `OnAppEndOperationCompletedAsync(string instanceId, string operation, object? result)` - Resume workflow after AppEnd operation
- `GetEventHandlers(string eventName)` - Get handlers for specific event
- `GetSubscribedEvents()` - List all subscribed events

**Supported Workflow Events**:
- `WorkflowStarted` - Fired when workflow execution begins
- `WorkflowCompleted` - Fired when workflow execution completes
- `WorkflowFaulted` - Fired when workflow encounters error
- `WorkflowSuspended` - Fired when workflow is suspended
- `WorkflowResumed` - Fired when workflow resumes
- `WorkflowCancelled` - Fired when workflow is cancelled
- `ActivityCompleted` - Fired when activity completes

#### `WorkflowEvent` (Model)
Represents a workflow event with context.

**Properties**:
- `EventName` - Name of the event
- `WorkflowInstanceId` - ID of the workflow instance
- `Source` - Component that triggered the event
- `Payload` - Event data (Dictionary<string, object>)
- `OccurredAt` - Timestamp of event

#### `WorkflowEventHandlers` (Static Helpers)
Built-in event handlers for common workflow events.

**Methods**:
- `OnWorkflowCompletedAsync()` - Handle workflow completion
- `OnWorkflowFaultedAsync()` - Handle workflow failure
- `OnActivityCompletedAsync()` - Handle activity completion

**Example Usage**:
```csharp
var eventSystem = serviceProvider.GetRequiredService<WorkflowEventSystemIntegration>();

// Subscribe to workflow completed event
await eventSystem.SubscribeToWorkflowEvent("WorkflowCompleted", async (@event) =>
{
    var instanceId = @event.WorkflowInstanceId;
    var payload = @event.Payload;
    
    // Handle completion logic
    await SendNotificationAsync($"Workflow {instanceId} completed");
});

// Subscribe to workflow faulted event
await eventSystem.SubscribeToWorkflowEvent("WorkflowFaulted", async (@event) =>
{
    var error = @event.Payload?["Error"]?.ToString();
    await LogErrorAsync($"Workflow failed: {error}");
});

// When AppEnd operation completes, resume the workflow
await eventSystem.OnAppEndOperationCompletedAsync(
    workflowInstanceId: "instance-123",
    operationName: "SendEmail",
    result: new { status = "sent", messageId = "msg-456" }
);
```

---

## Part 3: RPC Endpoints ✅

**File**: `AppEndServer/Workflows/Phase2/WorkflowRpcProxy.cs`

### Class: `WorkflowRpcProxy`
Exposes all workflow operations through AppEnd's RpcNet framework for client access.

**Key Features**:
- Wraps all workflow service operations in RPC-callable methods
- Comprehensive logging for debugging and auditing
- Error handling and exception propagation
- Organized into 3 sections: Execution, Definition Management, Instance Management

### Execution Operations

#### `ExecuteWorkflowAsync()`
Executes a workflow definition and returns the instance ID.

```csharp
// Call via RPC from client
var instanceId = await workflowProxy.ExecuteWorkflowAsync(
    workflowDefinitionId: "approval-workflow-001",
    input: new Dictionary<string, object> { { "amount", 5000 } },
    correlationId: "batch-123"
);
```

#### `ResumeWorkflowAsync()`
Resumes a suspended workflow instance.

```csharp
await workflowProxy.ResumeWorkflowAsync(
    workflowInstanceId: "instance-456",
    input: new Dictionary<string, object> { { "approved", true } }
);
```

#### `SuspendWorkflowAsync()`
Suspends a running workflow instance.

```csharp
await workflowProxy.SuspendWorkflowAsync(
    workflowInstanceId: "instance-456",
    reason: "Waiting for external approval"
);
```

#### `CancelWorkflowAsync()`
Cancels a workflow instance.

```csharp
await workflowProxy.CancelWorkflowAsync(workflowInstanceId: "instance-456");
```

### Definition Management Operations

#### `GetWorkflowDefinitionAsync()`
Retrieves workflow definition by ID.

#### `GetWorkflowDefinitionByNameAsync()`
Retrieves workflow definition by name.

#### `ListWorkflowDefinitionsAsync()`
Lists all workflow definitions.

#### `CreateWorkflowDefinitionAsync()`
Creates a new workflow definition.

#### `UpdateWorkflowDefinitionAsync()`
Updates an existing workflow definition.

#### `PublishWorkflowDefinitionAsync()`
Publishes a workflow definition (marks as ready for execution).

#### `DeleteWorkflowDefinitionAsync()`
Deletes a workflow definition.

### Instance Management Operations

#### `GetWorkflowInstanceAsync()`
Retrieves workflow instance by ID.

#### `GetWorkflowInstancesByCorrelationIdAsync()`
Retrieves all instances with given correlation ID.

#### `ListWorkflowInstancesAsync()`
Lists workflow instances with filtering and pagination.

#### `GetWorkflowExecutionHistoryAsync()`
Gets execution history (events) for an instance.

#### `GetWorkflowActivityExecutionsAsync()`
Gets activity executions for an instance.

**Example Registration in Program.cs**:
```csharp
builder.Services.AddScoped<WorkflowRpcProxy>();
```

---

## Part 4: Workflow Execution Engine ✅

**File**: `AppEndServer/Workflows/Phase2/WorkflowExecutionEngine.cs`

### Class: `WorkflowExecutionEngine`
Core workflow execution engine implementing the lifecycle of workflow instances.

**Design**:
- Delegates Elsa-specific logic to TODO markers for later implementation
- Handles all lifecycle states: Created, Running, Suspended, Completed, Faulted, Cancelled
- Publishes events to the workflow event system
- Validates state transitions
- Comprehensive error handling and logging

### Execution Operations

#### `ExecuteWorkflowAsync()`
**Steps**:
1. Validate workflow definition exists and is published
2. Create workflow instance (TODO: Elsa implementation)
3. Start execution (TODO: Elsa implementation)
4. Publish `WorkflowStarted` event

**Returns**: Workflow instance ID

#### `ResumeWorkflowAsync()`
**Steps**:
1. Validate instance exists and is suspended
2. Resume execution (TODO: Elsa implementation)
3. Publish `WorkflowResumed` event

#### `SuspendWorkflowAsync()`
**Steps**:
1. Validate instance exists and is running
2. Suspend execution (TODO: Elsa implementation)
3. Publish `WorkflowSuspended` event

#### `CancelWorkflowAsync()`
**Steps**:
1. Validate instance exists and is not in terminal state
2. Cancel execution (TODO: Elsa implementation)
3. Publish `WorkflowCancelled` event

#### `CompleteWorkflowAsync()`
**Steps**:
1. Validate instance exists
2. Mark as completed (TODO: Elsa implementation)
3. Publish `WorkflowCompleted` event with output data

#### `FaultWorkflowAsync()`
**Steps**:
1. Validate instance exists
2. Mark as faulted (TODO: Elsa implementation)
3. Publish `WorkflowFaulted` event with error details
4. No exception thrown (non-blocking)

**Example Usage**:
```csharp
var engine = serviceProvider.GetRequiredService<WorkflowExecutionEngine>();

// Execute workflow
var instanceId = await engine.ExecuteWorkflowAsync(
    workflowDefinitionId: "approval-workflow-001",
    input: new Dictionary<string, object> { { "amount", 5000 } }
);

// Later, suspend it
await engine.SuspendWorkflowAsync(instanceId);

// Resume when ready
await engine.ResumeWorkflowAsync(instanceId, 
    new Dictionary<string, object> { { "approved", true } }
);

// Or cancel it
await engine.CancelWorkflowAsync(instanceId);
```

---

## Integration Architecture

```
┌─────────────────────────────────────────────────────────┐
│                   AppEnd Application                    │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌─────────────────────┐     ┌──────────────────────┐  │
│  │   RPC Endpoints     │     │   Scheduler Service  │  │
│  │ (WorkflowRpcProxy)  │     │  (Background Tasks)  │  │
│  │                     │     │                      │  │
│  │ - Execute           │     │ - Cron-based Tasks   │  │
│  │ - Resume            │     │ - Task Management    │  │
│  │ - Suspend           │     │ - Periodic Execution │  │
│  │ - Cancel            │     │                      │  │
│  │ - Query Instances   │     └──────────────────────┘  │
│  │ - Manage Definitions│              ▲                │
│  │                     │              │                │
│  └──────────┬──────────┘              │                │
│             │                 ┌───────┴────────┐       │
│             │                 │ Scheduler      │       │
│             │                 │ Integration    │       │
│             │                 │                │       │
│             │                 │ - Register     │       │
│             │                 │ - Pause/Resume │       │
│             │                 │ - Unregister   │       │
│             │                 └────────────────┘       │
│             │                                          │
│             ▼                                          │
│  ┌─────────────────────────────────────────────────┐  │
│  │        Workflow Execution Engine                │  │
│  │   (Core Workflow Lifecycle Manager)             │  │
│  │                                                 │  │
│  │ - ExecuteWorkflowAsync()                        │  │
│  │ - ResumeWorkflowAsync()                         │  │
│  │ - SuspendWorkflowAsync()                        │  │
│  │ - CancelWorkflowAsync()                         │  │
│  │ - CompleteWorkflowAsync()                       │  │
│  │ - FaultWorkflowAsync()                          │  │
│  │                                                 │  │
│  │ [TODO: Elsa workflow execution logic]           │  │
│  └──────────────────┬──────────────────────────────┘  │
│                     │                                  │
│                     ▼                                  │
│  ┌──────────────────────────────────────────────────┐ │
│  │    Event System Integration                      │ │
│  │  (Workflow Event Pub/Sub & AppEnd Coordination) │ │
│  │                                                  │ │
│  │ - PublishWorkflowEventAsync()                   │ │
│  │ - SubscribeToWorkflowEvent()                    │ │
│  │ - OnAppEndOperationCompletedAsync()             │ │
│  │                                                  │ │
│  │ Supported Events:                               │ │
│  │ - WorkflowStarted                               │ │
│  │ - WorkflowCompleted                             │ │
│  │ - WorkflowFaulted                               │ │
│  │ - WorkflowSuspended/Resumed                     │ │
│  │ - WorkflowCancelled                             │ │
│  │ - ActivityCompleted                             │ │
│  └──────────────────┬───────────────────────────────┘ │
│                     │                                  │
│                     ▼                                  │
│  ┌──────────────────────────────────────────────────┐ │
│  │    Workflow Service Layer (Phase 1)              │ │
│  │                                                  │ │
│  │ - IWorkflowService (Facade)                     │ │
│  │ - IWorkflowDefinitionService                    │ │
│  │ - IWorkflowInstanceService                      │ │
│  └──────────────────┬───────────────────────────────┘ │
│                     │                                  │
└─────────────────────┼──────────────────────────────────┘
                      │
                      ▼
              ┌──────────────────┐
              │  Elsa 3.0 Engine │
              │  [TODO: Integrate]
              │                  │
              │ - Workflow Exec  │
              │ - Bookmarks      │
              │ - Activities     │
              │ - Persistence    │
              └──────────────────┘
```

---

## DI Registration

Add to `Program.cs` `ConfigServices()` method:

```csharp
// Register Phase 2 integration components
builder.Services.AddScoped<WorkflowSchedulerIntegration>();
builder.Services.AddScoped<WorkflowEventSystemIntegration>();
builder.Services.AddScoped<WorkflowExecutionEngine>();
builder.Services.AddScoped<WorkflowRpcProxy>();

// Initialize the scheduler executor with DI services
var serviceProvider = builder.Services.BuildServiceProvider();
var workflowService = serviceProvider.GetRequiredService<IWorkflowService>();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
WorkflowScheduledExecutor.Initialize(workflowService, loggerFactory);
```

---

## Testing Checklist

- [ ] WorkflowSchedulerIntegration registers workflows as scheduled tasks
- [ ] WorkflowScheduledExecutor executes from scheduler at scheduled time
- [ ] WorkflowEventSystemIntegration publishes events correctly
- [ ] Event subscribers are called when events are published
- [ ] WorkflowRpcProxy RPC methods callable from client
- [ ] WorkflowExecutionEngine validates state transitions
- [ ] Events published after each state change
- [ ] Error handling works for invalid operations
- [ ] Logging captures all operations for debugging

---

## Next Steps

### Phase 3: Custom Activities
Implement workflow activities:
- Database activity (query, insert, update, delete)
- DynaCode activity (execute dynamic C# code)
- Notification activity (email, SMS, notifications)
- HTTP request activity
- Approval activity (human task with AppEnd integration)

### Phase 4: Operations & UI
- Elsa embedded workflow designer
- Workflow instance dashboard
- Activity monitoring
- Execution history view
- Error tracking and debugging

### Elsa Package Installation
When ready, install NuGet packages:
```bash
dotnet add package Elsa --version 3.0.0
dotnet add package Elsa.Persistence.EntityFrameworkCore.SqlServer --version 3.0.0
dotnet add package Elsa.Activities.Temporal --version 3.0.0
```

Then uncomment Elsa code in `WorkflowServices.cs` and implement TODO placeholders.

---

## Build Status

✅ **Phase 1 (Foundation)**: Complete and compiling
✅ **Pre-Phase 2 (Setup)**: Complete
✅ **Phase 2 Part 1 (Scheduler)**: Complete and compiling
✅ **Phase 2 Part 2 (Events)**: Complete and compiling
✅ **Phase 2 Part 3 (RPC)**: Complete and compiling
✅ **Phase 2 Part 4 (Execution)**: Complete and compiling

**Overall Status**: Phase 2 - COMPLETE ✅

All components compile successfully and are ready for Elsa package installation and TODO implementation.
