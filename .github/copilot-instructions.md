# Copilot Instructions

## Project Guidelines
- When adding styles for Vue components, prefer reusable/global CSS instead of component-scoped `<style>` blocks: if the styles are AppEndStudio-specific, place them in `AppEndStudio/assets/custom.css`; if they are reusable across apps/components, place them under `a..lib/append/css` (e.g., `append-components.css`).
- When writing code in this repo, comments must be in English, and new Vue components should follow the standards/layout patterns used in `AppEndStudio/components`. Additionally, responses should be provided in Persian when applicable.
- When implementing resizable split panes, use the existing `flex-splitter` plugin/pattern already used in the project (e.g., `data-flex-splitter-*` attributes) instead of custom drag handlers.

## Modal and Window Management Guidelines
- **NEVER** create inline modals within Vue components using manual HTML (e.g., `<div class="modal d-block">...</div>`).
- **ALWAYS** create a separate Vue component for modal content and use the `openComponent()` function from `append-window-manager.js` to display it.
- For each modal/dialog/window, create a dedicated component file (e.g., `ComponentNameEditor.vue`, `ComponentNameDetails.vue`, `ComponentNameForm.vue`).
- Modal components should:
  - Accept parameters via `shared["params_" + cid]` in their `setup()` function
  - Use `closeComponent(cid, resultObject)` to close themselves and return results to the caller
  - Implement callback patterns via `caller` and `callback` options in `openComponent()`
- Common `openComponent()` options:
  - `modal: true` - Opens as Bootstrap modal
  - `modalSize: "modal-lg" | "modal-xl" | "modal-fullscreen"` - Modal size
  - `windowSizeSwitchable: true` - Shows maximize/restore button
  - `params: { ... }` - Parameters to pass to the opened component
  - `caller: this` - Reference to calling component
  - `callback: function(result) { ... }` - Callback function for handling results
- Example pattern:
  ```javascript
  openComponent("/path/to/Component", {
      title: "Component Title",
      modalSize: "modal-lg",
      windowSizeSwitchable: true,
      params: { data: someData },
      caller: this,
      callback: function(result) {
          if (result?.success) {
              this.reloadData();
          }
      }
  });
  ```

## Loading State Management Guidelines
- **DO NOT** create manual loading states/spinners in Vue components (e.g., `loading: true`, `<i class="fas fa-spinner fa-spin">`, `:disabled="loading"`).
- The AppEnd framework automatically handles loading indicators for:
  - RPC calls (`rpcAEP()`)
  - Component loading
  - Modal opening/closing
- Simply call `rpcAEP()` without worrying about loading states - the framework will show appropriate loading indicators.
- Exception: You may use `v-if` to conditionally show content based on data availability (e.g., `v-if="items.length === 0"`), but not for loading states.

## List Component Guidelines
- **ALWAYS** follow the standard list component pattern used in AppEnd (e.g., `DbDbObjects.vue`).
- List components should have:
  - **Header** (`card-header` with `bg-body-subtle`):
    - Filter inputs (search, dropdowns) using `form-control-sm` and `form-select-sm`
    - Separator with `<div class="vr"></div>` between filters
    - Search/refresh button with `btn-outline-primary`
    - `<div class="p-0 ms-auto"></div>` before action buttons
    - Primary action button (Create/Add) with `btn-primary` on the right
  - **Body** (`card-body` with `scrollable`):
    - Use `table` with classes: `table table-sm table-hover w-100 ae-table m-0 bg-transparent`
    - Headers with `sticky-top ae-thead-th text-dark fw-bold`
    - First column should be clickable link to edit with classes: `text-secondary text-hover-primary text-decoration-none`
    - Action buttons in last column with `btn-sm btn-outline-*` classes
- **Script structure** (in this exact order):
  1. Initialize `_this` object with data arrays and filters
  2. `methods` section
  3. `computed` section (for filtering)
  4. `setup(props)` section
  5. `data()` section - return references to `_this` properties
  6. `created()` section
  7. `mounted()` section
  8. `props` definition
- Use `shared.showConfirm()` for confirmations instead of native `confirm()`
- Use `_.filter()` from lodash for filtering arrays
- Use `shared.fixNull()` to safely check nullable values
- Reference pattern:
  ```vue
  let _this = { cid: "", c: null, items: [], filter: {} };
  _this.filter = { search: "", status: "" };
  
  data() {
      return {
          items: _this.items,
          filter: _this.filter
      };
  }
  ```

## Documentation Guidelines
- Do not automatically create summary/documentation files after code changes. Only create documentation when explicitly requested.