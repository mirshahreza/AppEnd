# Copilot Instructions

## Project Guidelines
- When adding styles for Vue components, prefer reusable/global CSS instead of component-scoped `<style>` blocks: if the styles are AppEndStudio-specific, place them in `AppEndStudio/assets/custom.css`; if they are reusable across apps/components, place them under `a..lib/append/css` (e.g., `append-components.css`).
- When writing code in this repo, comments must be in English, and new Vue components should follow the standards/layout patterns used in `AppEndStudio/components`.