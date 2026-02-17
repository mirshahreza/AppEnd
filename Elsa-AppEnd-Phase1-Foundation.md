# Phase 1 - Foundation

## Objectives
- Add Elsa core runtime to AppEnd
- Configure hosting and persistence
- Prove execution of a minimal workflow

## Deliverables
- Elsa packages referenced in `AppEndServer`
- Service registration in `AppEndHost` startup
- Workflow persistence configured
- Sample workflows for both code-first and database/JSON definitions
- Sample workflow set (to add):
  - Code-first Hello World (Sequence + Delay)
  - Database/JSON Hello World (designer-defined)
  - Cron-triggered workflow (scheduler integration)
  - Event-triggered workflow (AppEnd event/RPC)
  - Long-running approval with persistence (Delay/Wait + resume)
  - Sub-workflow invocation (parent/child)
  - Failure + retry/compensation path

## Key Architectural Decisions (Phase 1)
- **Service Layer Pattern**: All workflow execution requests flow through a centralized `IWorkflowService` interface, decoupling consumers (UI, schedulers, events) from the Elsa engine
- **Dual Definition Model**: Both code-first and database/JSON workflow definitions are supported from the start to unblock both use cases
- **Error Handling**: Leverage Elsa's built-in retry, compensation, and fault handling mechanisms; document patterns in sample workflows

## Out of Scope
- Custom activities
- Visual designer integration
- Admin UI

## Open Questions
- Which storage provider aligns with AppEnd DB usage?
- How to store workflow definitions (code-first vs. database)?

## Acceptance Criteria
- App starts with Elsa services registered
- A basic workflow can run and be persisted
