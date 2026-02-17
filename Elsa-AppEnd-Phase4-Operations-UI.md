# Phase 4 - Operations & UI

## Objectives
- Provide admin/ops visibility into workflows
- Offer a UI for authoring and monitoring workflows

## Deliverables
- Monitoring endpoints/log views (see Elsa-AppEnd-Monitoring-Observability-Strategy.md)
  - Workflow execution metrics (success/failure rates, durations)
  - Real-time dashboard (active instances, current activities)
  - Execution history and detail views
  - Audit trail (user, timestamp, parameters, results)
  
- Basic management UI (start/stop, view status)
  - Workflow definition library
  - Manual execution trigger with input parameters
  - Instance management (cancel, resume suspended)
  - Error analysis and failure diagnostics
  
- Optional Elsa Studio integration
  - Visual workflow designer (if adopted)
  - Alternative to custom AppEnd authoring UI

## Monitoring & Observability
- Leverage Elsa's event system (WorkflowExecuting, ActivityExecuted, Faulted)
- Integrate with AppEnd logging framework
- Support real-time alerts for timeout, failures, suspension
- Capture full execution history for replay/debugging

## Out of Scope
- Full AppEnd workflow marketplace
- Advanced analytics and reporting

## Open Questions
- Embed Elsa Studio vs. custom AppEnd pages?
- Required authentication/authorization model?

## Acceptance Criteria
- Operators can view workflow status and history
- A visual authoring path is available (studio or custom)
