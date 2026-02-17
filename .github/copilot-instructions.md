# Copilot Instructions

## Project Guidelines
- When adding styles for Vue components, prefer reusable/global CSS instead of component-scoped `<style>` blocks: if the styles are AppEndStudio-specific, place them in `AppEndStudio/assets/custom.css`; if they are reusable across apps/components, place them under `a..lib/append/css` (e.g., `append-components.css`).
- When writing code in this repo, comments must be in English, and new Vue components should follow the standards/layout patterns used in `AppEndStudio/components`.
- When implementing resizable split panes, use the existing `flex-splitter` plugin/pattern already used in the project (e.g., `data-flex-splitter-*` attributes) instead of custom drag handlers.

## Implementation Workflow
- Focus on core implementation and skip non-essential steps (e.g., database migrations).
- Follow the sequential progression through implementation phases: Phase 1-4 Implementation → Integration Testing → API Testing → UI Integration → End-to-End Testing → Production. 
- Work in Persian language for all documentation and comments.