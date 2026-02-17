# Elsa + AppEnd - Plan Overview

## Goal
Integrate Elsa workflows with AppEnd to enable configurable, long-running business processes with minimal disruption to current architecture.

## Objectives
- **Workflow Engine**: Add a reliable, persistent workflow execution system to AppEnd
- **Hybrid Automation**: Support both automated (machine) and manual (human) approval/decision steps
- **Complete Integration**: Seamless integration at both code layer (custom activities, events) and UI layer (embedded designer, management dashboard)
- **Visual Design**: Leverage Elsa's standard designer for workflow authoring
- **AppEnd Branding**: Customize UI/styling to show AppEnd identity; Elsa branding hidden
- **Single Deployment**: Run Elsa within the main AppEnd web application (no separate service initially)

## Scope
- Elsa engine integration with AppEnd server runtime
- Workflow definition, execution, and persistence
- Custom activities for AppEnd features
- Embedded Elsa Studio with AppEnd branding and customization
- Optional future extraction to separate service

## Assumptions
- Target framework: .NET 10
- Initial deployment within `AppEndServer` (no new project)
- Use existing AppEnd logging, configuration, and scheduling patterns
- UI embedded in main AppEnd web app (Phase 4)

## Architecture Decision (Initial)
- **Start**: Add Elsa services and storage into `AppEndServer`
- **Later**: Extract to a separate service if load/scale demands

## Key Architectural Principles
- **Service Layer Decoupling**: All workflow consumers (UI, schedulers, events) interact through a stable `IWorkflowService` interface, not directly with Elsa
- **Dual Definition Model**: Support both code-first (C# versioned) and database/JSON (dynamic) workflow definitions
- **Monitoring-First**: Leverage Elsa's native event system to provide comprehensive execution tracking, audit, and observability
- **UI Customization**: Embed Elsa Studio with AppEnd theme/branding; hide Elsa branding (Phase 4)

## Phases
1. **Foundation**: packages, hosting, storage, basic runtime wiring
2. **Integration**: AppEnd-facing workflow APIs, scheduling hooks, service layer
3. **Custom Activities**: AppEnd actions (DB, DynaCode, notifications)
4. **Operations & UI**: embedded designer with branding, monitoring dashboard, optional Elsa Studio customization
5. **Documentation**: comprehensive user/developer guides, API reference, sample workflows, deployment guide

## Open Decisions
- **Storage Provider**: SQL Server vs. existing DB abstraction (Phase 1)
- **Workflow Definition Format**: Confirm code-first vs. database/JSON strategy (Phase 1)
- **Security & Authorization**: User roles, execution permissions, data access, custom activity sandboxing (see Elsa-AppEnd-Security-Authorization-Open-Decisions.md)
- **UI Customization Details**: CSS theming strategy, navbar integration, custom icons (Phase 4 - see Elsa-AppEnd-UI-Branding-Customization.md)

## Supporting Documentation
- **Elsa-AppEnd-Phase1-Foundation.md**: Packages, service registration, sample workflows
- **Elsa-AppEnd-Phase2-Integration.md**: Service layer abstraction, scheduling integration
- **Elsa-AppEnd-Phase3-Custom-Activities.md**: DB, DynaCode, and notification activities
- **Elsa-AppEnd-Phase4-Operations-UI.md**: Monitoring, management UI, embedded designer
- **Elsa-AppEnd-Phase5-Documentation.md**: User/developer guides, API reference, samples, deployment
- **Elsa-AppEnd-UI-Branding-Customization.md**: Elsa Studio theming, branding strategy
- **Elsa-AppEnd-Security-Authorization-Open-Decisions.md**: Security questions to resolve
- **Elsa-AppEnd-Monitoring-Observability-Strategy.md**: Full monitoring implementation based on Elsa capabilities
- **Elsa-AppEnd-Remaining-Items-TBD.md**: Testing, deployment, documentation, rollback, and compliance items
- **Elsa-Documentation-Introduction-Summary.md**: Elsa 3.0 overview reference
