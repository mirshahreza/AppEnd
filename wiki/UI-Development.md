# UI Development

AppEnd uses a SPA built with Bootstrap and Vue.js 3. UIs can be generated and extended.

## UI generation
- Lists and forms are generated for each entity configured in a module
- Generated UIs are responsive by default

## Custom UI
- Create custom Vue components
- Override or extend generated forms and lists
- Add validation, conditional fields, and computed properties
- Use translation files to localize UI strings

## Styling
- Leverage Bootstrap utilities and components
- Keep custom CSS scoped to components to avoid leakage

## Internationalization (i18n)
- UI uses translation files to support multiple languages
- Provide locale files and switch language at runtime

## Patterns
- Keep components small and focused
- Use composition API and reusable composables where appropriate

See [[Modules]] and [[Configuration]].