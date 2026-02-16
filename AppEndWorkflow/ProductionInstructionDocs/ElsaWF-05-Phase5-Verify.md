# Phase 5 — Build & Verify

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## Goal
Ensure everything compiles and Elsa initializes correctly.

## Checklist
- [ ] `dotnet build` succeeds with no errors
- [ ] Application starts without exceptions
- [ ] Elsa tables exist in the database (created manually via `ElsaSchema.sql` before startup)
- [ ] `rpcAEP("ExecuteWorkflow", {DefinitionId:"hello-world"})` executes and returns result
- [ ] `rpcAEP("ExecuteWorkflow", {DefinitionId:"data-pipeline"})` executes batch processing and returns summary
- [ ] `rpcAEP("GetWorkflowDefinitions", {})` returns all 4 registered sample workflows
- [ ] Scheduled DB Check workflow auto-fires on its cron schedule when published
- [ ] Order Approval workflow suspends and creates kartabl task when triggered
- [ ] No new HTTP routes are exposed (only existing `talk-to-me` endpoint)
