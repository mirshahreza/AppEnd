# Copilot Instructions

## Project Guidelines
- When adding styles for Vue components, prefer reusable/global CSS instead of component-scoped `<style>` blocks: if the styles are AppEndStudio-specific, place them in `AppEndStudio/assets/custom.css`; if they are reusable across apps/components, place them under `a..lib/append/css` (e.g., `append-components.css`).
- Always write code comments and documentation in English only. Do not use Farsi or any other language in code unless explicitly requested by the user. However, for all conversations, communicate in Farsi (Persian) language.
- New Vue components should follow the standards/layout patterns used in `AppEndStudio/components`.
- When implementing resizable split panes, use the existing `flex-splitter` plugin/pattern already used in the project (e.g., `data-flex-splitter-*` attributes) instead of custom drag handlers.