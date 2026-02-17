# Phase 2 - Integration

## Objectives
- Expose workflow execution to AppEnd features
- Integrate with existing scheduling and logging

## Deliverables
- Workflow API/Service layer in `AppEndServer`
- Scheduling bridge (trigger workflows on cron/events)
- Consistent logging/telemetry
- Service layer abstraction: UI, schedulers, and event triggers communicate only through a stable internal API, not directly with Elsa engine

## Architectural Principles
- **Service Layer Decoupling**: All workflow execution requests (whether from UI, scheduling, or event triggers) must go through the stable `IWorkflowService` interface
  - Ensures engine agility (can replace/upgrade Elsa without changing consumers)
  - Single point for logging, error handling, and observability
  - Consistent API contract for all callers
  
## Open Questions & Decisions Needed
- API versioning strategy for `IWorkflowService` (see Elsa-AppEnd-Security-Authorization-Open-Decisions.md)
- Which triggers are priority (cron, RPC, events)?
- How should users manage workflow lifecycles (manual vs. automated)?

## Acceptance Criteria
- AppEnd can start a workflow via internal API
- Scheduler can trigger a workflow run
