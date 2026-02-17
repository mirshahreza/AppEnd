# Phase 3 - Custom Activities

## Objectives
- Provide reusable Elsa activities that map to AppEnd capabilities

## Deliverables
- Activities for DB operations
- Activities for DynaCode execution
- Activities for notifications and file operations

## Activity Priority (Recommended Order)
1. **Database Activity** (critical: data-driven workflows)
   - Execute SELECT/INSERT/UPDATE/DELETE
   - Use AppEnd's DB connection/transaction model
   
2. **DynaCode Activity** (powerful: custom business logic)
   - Execute AppEnd DynaCode snippets
   - Pass workflow variables as context
   
3. **Notification/File Activities** (operational: output/alerting)
   - Send notifications (email, webhook, etc.)
   - Write/read files in AppEnd context

## Security Considerations (See Elsa-AppEnd-Security-Authorization-Open-Decisions.md)
- DynaCode execution requires approval process/sandboxing
- Activities must validate inputs and respect AppEnd data access rules
- All activity inputs should be auditable

## Out of Scope
- Visual designer UI
- External marketplace of activities

## Open Questions
- Which AppEnd actions are most critical for first release?
- Security model for executing dynamic code in workflows?

## Acceptance Criteria
- At least 3 custom activities work end-to-end in a workflow
