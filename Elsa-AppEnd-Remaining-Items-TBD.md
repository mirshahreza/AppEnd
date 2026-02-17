# Elsa + AppEnd - Remaining Items (TBD)

## Overview
This document tracks all items that are critical but deferred to later phases or require detailed planning.

---

## 1. Testing Strategy

### Unit Tests
- Test individual activities (Database, DynaCode, Notification)
- Mock Elsa runtime and persistence layer
- Test error handling, retry logic, compensation

### Integration Tests
- End-to-end workflow execution with real database
- Verify service layer (`IWorkflowService`) contracts
- Test scheduling triggers and event handling
- Test persistence (save/resume workflows)

### Performance/Load Tests
- Concurrent workflow execution (10, 100, 1000+ instances)
- Large variable payloads
- Long-running workflow memory behavior
- Activity execution throughput

### Acceptance Tests
- Sample workflows from Phase 1 (code-first and DB-defined)
- Custom activity workflows from Phase 3
- Scheduler integration and event triggers

### Test Data & Fixtures
- Sample workflow definitions (JSON, C#)
- Mock AppEnd data (users, configs, DynaCode snippets)
- Failure scenarios (timeout, bad input, permission denied)

---

## 2. Performance & Scalability

### Capacity Planning
- **Current Assumption**: Initial AppEnd deployment scale
- **Estimation Needed**: Expected workflow count, concurrent executions, data volume
- **Database Bottleneck**: Query performance for workflow queries, activity logs, resume points
- **Memory Usage**: Workflow instance state size, bookmark overhead

### Tuning Points
- Persistence batch size (how many runs per save)
- Event subscription overhead
- Activity execution timeouts and defaults
- In-memory caching strategy (if any)

### Monitoring Metrics
- Workflow execution latency (from trigger to completion)
- Activity execution time per type
- Queue depth (pending workflows)
- Database query time for common operations
- Memory footprint per active workflow

### Scalability Strategy
- Vertical scaling: Single AppEndServer instance limits?
- Horizontal scaling: Can workflows run on multiple servers? (deferred to Phase 4+)
- Database scaling: Partitioning or read replicas needed?

---

## 3. Deployment & Infrastructure

### Database Setup
- **Migrations**: SQL scripts to create workflow tables (Elsa schema)
- **Indexes**: Performance indexes on common queries (workflow ID, status, created date)
- **Backups**: Strategy for workflow instance data
- **Versioning**: How to migrate storage schema across Elsa versions

### AppEndServer Changes
- NuGet package additions (Elsa.Core, Elsa.EntityFrameworkCore, etc.)
- Startup configuration (Elsa service registration, persistence setup)
- Configuration files (connection strings, activity settings)
- Logging configuration (route Elsa logs to AppEnd logger)

### Build & Release
- Update project file (.csproj) to reference Elsa packages
- CI/CD: Ensure builds include Elsa types and dependencies
- Release notes: Document new workflow capabilities
- Documentation links in deployment guides

### Environment Configuration
- Development: Local SQLite or SQL Server Express
- Staging: Shared test database
- Production: Dedicated database with HA/backup

### Infrastructure Monitoring
- Database disk space (for workflow logs/instances)
- AppEndServer CPU/memory with workflows enabled
- Persistence layer health checks

---

## 4. Documentation for Users

### Workflow Author Guide
- **Getting Started**: Create your first workflow (code vs. designer)
- **Activity Reference**: All built-in and custom activities (DB, DynaCode, etc.)
- **Common Patterns**: Error handling, retry, approval workflows, long-running tasks
- **Best Practices**: Performance tips, avoid blocking operations, security considerations
- **Troubleshooting**: Common errors, logs, debugging techniques

### API Documentation
- `IWorkflowService` contract (start, cancel, get status, etc.)
- Event/webhook integration points
- Scheduler integration examples
- Input/output passing conventions

### Example Workflows
- Simple hello world (code and designer)
- Database query + notification workflow
- Long-running approval (wait/bookmark pattern)
- Error handling with compensation
- Sub-workflow invocation

### FAQs
- How do I pass data to a workflow?
- Can I run the same workflow multiple times?
- How long can a workflow run?
- Can workflows talk to each other?
- What happens if a server crashes mid-workflow?

---

## 5. Training & Onboarding

### Developer Training
- **Elsa Concepts**: Activities, workflows, bookmarks, storage providers
- **AppEnd Integration**: How Elsa fits into the stack
- **Custom Activity Development**: Template and guidelines
- **Testing Patterns**: How to test workflows locally

### Operator/Admin Training
- **Monitoring Dashboard**: How to use Phase 4 UI
- **Troubleshooting Workflows**: Reading logs, understanding failure reasons
- **Performance**: When to scale, when to optimize
- **Disaster Recovery**: How to resume workflows after outage

### Knowledge Base
- Wiki or internal documentation site
- Video tutorials (optional, for Phase 4+)
- Code samples repository (within AppEnd.git or separate)
- Q&A forum or support channel

---

## 6. Rollback Strategy

### Pre-Deployment
- Database backup before Elsa migration
- AppEndServer binaries backup
- Configuration backup (web.config, appsettings.json)

### Rollback Scenarios
- **New Elsa Package**: Revert NuGet version, restart service, verify DB schema compatibility
- **Database Migration Failed**: Restore from backup, investigate schema
- **Workflow Definitions Broken**: Revert definitions from version control
- **Critical Bug**: Disable workflows (feature flag), fix, re-enable

### Testing Rollback
- Test rollback process in staging
- Document manual steps if needed
- Estimate rollback time for SLA

### Data Consistency
- What happens to in-flight workflows during rollback?
- Can workflow state be recovered from previous backup?
- Transaction logs for audit trail

---

## 7. Integration Map

### Connection Points with AppEnd

#### AppEndServer Startup (AppEndHost)
- Register Elsa services in DI container
- Configure database persistence
- Start Elsa runtime

#### AppEnd Scheduler/Cron System
- Bridge to trigger workflows on schedule
- Hook into existing cron/job execution

#### AppEnd Event System (if exists)
- Publish workflow events (start, complete, fail)
- Subscribe to AppEnd events to trigger workflows

#### AppEnd Logging
- Route Elsa diagnostic logs to AppEnd logger
- Use same log levels, correlation IDs, context

#### AppEnd Authorization
- Check user roles before allowing workflow actions
- Respect AppEnd permission model for custom activities

#### AppEnd Database
- Share database instance vs. separate?
- Use AppEnd's DB connection/transaction pattern
- Integrate with AppEnd's data layer

#### AppEnd UI (AppEndStudio or web app)
- Expose workflow management endpoints
- Add workflow section to navigation
- Visual designer (Elsa Studio embed or custom)

---

## 8. Migration Path

### If Workflows Exist Elsewhere
- **Current State**: Where are existing workflows today?
  - Custom scheduler jobs?
  - Distributed as code modules?
  - In a different workflow engine?
  
- **Migration Strategy**:
  - Analyze existing workflows (structure, triggers, activities)
  - Map to Elsa equivalents
  - Rewrite or auto-convert (if possible)
  - Test against historical data
  - Parallel run period (old + new together)
  - Cutover and archive old system

- **Data Migration**:
  - Execution history (import or archive?)
  - In-flight executions (replay or abandon?)
  - Configuration/parameters (translate to workflow variables)

### Phased Rollout
- Pilot: Start with non-critical workflows
- Monitor: Track performance and reliability
- Expand: Gradually migrate more workflows
- Archive: Decommission old system

---

## 9. Accessibility & Localization (Optional for Phase 1)

### UI Accessibility
- WCAG 2.1 compliance for workflow UI
- Screen reader support for dashboards

### Localization
- Multi-language UI (if AppEnd supports)
- Error messages and help text in multiple languages

---

## 10. Compliance & Audit

### Data Governance
- Personal data handling in workflow context
- GDPR/compliance for data retention
- Right to deletion for workflow instances

### Audit Requirements
- Complete audit log of workflow definitions and executions
- Change tracking for workflow versions
- User action attribution (who ran what, when)

### Regulatory Reporting
- Can we generate compliance reports from workflow logs?
- Data retention policies aligned with business requirements

---

## Timeline & Dependencies

### Phase 1 (Foundation)
- **Must Have**: None of TBD items block Phase 1
- **Nice to Have**: Basic test framework, initial performance estimates
- **Post-Phase 1**: Finalize security model, deployment scripts

### Phase 2 (Integration)
- **Must Have**: Testing strategy, integration with AppEnd logging/auth
- **Post-Phase 2**: User documentation draft, training materials

### Phase 3 (Custom Activities)
- **Must Have**: Custom activity testing patterns, security guidelines
- **Post-Phase 3**: Activity reference documentation

### Phase 4 (Operations & UI)
- **Must Have**: Full monitoring & observability, rollback procedures
- **Post-Phase 4**: Ops documentation, admin training materials

---

## Summary Table

| Item | Priority | Phase | Owner | Status |
|------|----------|-------|-------|--------|
| Testing Strategy | High | 1-2 | Dev Lead | TBD |
| Performance Baseline | High | 1 | DevOps/Tech Lead | TBD |
| Database Migrations | High | 1 | DBA/Backend | TBD |
| Deployment Checklist | High | 1-2 | DevOps | TBD |
| User Documentation | Medium | 2-3 | Tech Writer | TBD |
| Rollback Procedures | High | 1 | DevOps | TBD |
| Integration Map | High | 1 | Architect | TBD |
| Training Materials | Medium | 2-4 | Tech Writer/SME | TBD |
| Migration Strategy | Low | 4+ | Product/Backend | TBD |
| Compliance Audit | Medium | 3-4 | Compliance/Security | TBD |
