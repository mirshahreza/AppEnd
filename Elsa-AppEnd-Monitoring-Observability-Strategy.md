# Monitoring & Observability - Elsa Engine Capabilities

## Overview
This document defines the monitoring and observability strategy based on what Elsa 3.0 natively provides.

## Elsa 3.0 Built-in Capabilities

### Workflow Execution Events
Elsa provides lifecycle events for each workflow execution:
- **WorkflowExecuting**: Fired when a workflow starts
- **WorkflowExecuted**: Fired when a workflow completes (success or failure)
- **ActivityExecuting**: Fired when an activity starts
- **ActivityExecuted**: Fired when an activity completes
- **Faulted**: Fired when an exception occurs

### Workflow Instance Tracking
- Each workflow instance has a unique ID
- Execution status: Running, Completed, Failed, Suspended
- Execution history: All activity executions in sequence
- Variables and outputs captured at each step
- Timestamps for all state transitions

### Persistence Data
From storage, we can query:
- Workflow definition metadata (name, version, activities)
- Workflow instance state (variables, bookmarks, current activity)
- Execution logs (activity results, timing, outputs)
- Bookmark information (for resumable/awaiting workflows)

## Comprehensive Monitoring Implementation

### 1. Execution Metrics
- **Per-workflow**:
  - Total executions count
  - Success/failure rates
  - Average duration
  - Peak execution times
  
- **Per-activity**:
  - Execution count
  - Average duration
  - Error rate
  - Most common inputs/outputs (if relevant)

### 2. Real-time Observability
- Active workflow instances (running count)
- Activity execution in progress
- Backlog/queued workflows
- Suspended workflows awaiting resume

### 3. Historical Analysis
- Workflow execution timeline (when, how long, result)
- Activity execution sequence (critical path analysis)
- Error stack traces and failure reasons
- Variable snapshots at each step (for debugging)

### 4. Alerting Triggers
- Workflow execution exceeded timeout
- Activity failed (configurable retry exhausted)
- High error rate for a specific workflow
- Long-running workflow detected
- Workflow suspended with no resume trigger

### 5. Audit Trail
- Who triggered the workflow (user/API caller)
- Input parameters passed
- All activity executions and results
- Any manual interventions (resume, cancel, etc.)

### 6. Dashboard/UI Components
- Workflow definition library (list, versions, statistics)
- Live execution dashboard (active instances, current activities)
- Execution history (list, filter by status/time/workflow)
- Instance detail view (full execution log, variable state)
- Error analysis (most common failures, failure patterns)

## Integration with AppEnd Logging
- Route Elsa events to AppEnd's logging framework
- Use consistent timestamp, correlation ID, and context
- Support AppEnd's log levels (Debug, Info, Warning, Error)
- Archive execution logs according to AppEnd retention policy

## Implementation Notes
- Subscribe to Elsa's event system (WorkflowExecuting, ActivityExecuted, etc.)
- Query the persistence store for historical data
- Aggregate metrics in memory or use AppEnd's existing metrics system
- Expose data via API for UI consumption
