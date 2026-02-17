# Security & Authorization - Open Decisions

## Scope
Define access control and security policies for workflow management in AppEnd + Elsa integration.

## Questions to Resolve

### 1. Workflow Definition Management
- Who can create/edit/delete workflow definitions?
- Should it be role-based (e.g., admin, developer, operator)?
- Are workflow definitions version-controlled or managed through UI?

### 2. Workflow Execution
- Who can trigger workflow execution (via API, scheduler, events)?
- Should execution be rate-limited or queued per user/tenant?
- Can a user cancel their own running workflows?

### 3. Workflow Data & Context
- What input data can be passed to workflows?
- Should workflows be isolated per tenant/organization?
- Can workflows access sensitive AppEnd data (user data, DynaCode)?

### 4. Custom Activities Security
- Can custom activities run arbitrary code (e.g., DynaCode activities)?
- Should there be a sandboxing mechanism or approval process?
- How do we prevent malicious workflow definitions from executing?

### 5. Audit & Compliance
- Should all workflow executions be logged with user/timestamp/result?
- Do we need encryption for stored workflow definitions?
- What data retention policies apply?

### 6. Service Layer API Security
- Should the internal `IWorkflowService` API have authentication/authorization?
- How do we secure communication between UI, schedulers, and the service layer?

## Tentative Approach (To Be Confirmed)
- Use AppEnd's existing authorization framework (roles, permissions)
- Extend it to cover workflow definition and execution actions
- Audit all execution attempts and results
- Document custom activity security guidelines
