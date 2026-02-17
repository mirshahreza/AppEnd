# UI Branding & Customization Strategy

## Overview
Embed Elsa Studio within the AppEnd web application with AppEnd branding and styling, hiding Elsa identity from end users.

## Objectives
- Seamless visual integration with AppEnd UI
- AppEnd branding (colors, logo, typography) throughout
- Hide Elsa branding (logo, watermarks, "Powered by Elsa" text)
- Single sign-on and consistent authentication
- Consistent navigation and layout with AppEnd shell

## Approach: Elsa Studio Embedded + Custom Branding

### 1. Embedding Elsa Studio
- **Host Elsa Studio** as a route within AppEnd web application (e.g., `/workflows/designer`)
- **Authentication**: Use AppEnd's session/auth middleware to protect access
- **Layout Wrapper**: Wrap Elsa Studio in AppEnd's shell (navbar, sidebar, footer)
- **Integration**: Link workflow management from main AppEnd dashboard

### 2. CSS Theming & Styling
- **Override Elsa Defaults**:
  - Base colors (primary, secondary, accent) → AppEnd palette
  - Fonts → AppEnd typography
  - Logo/branding → Replace Elsa logo with AppEnd logo
  - Icons → Use AppEnd icon set where applicable
  
- **CSS Strategy** (per Copilot Instructions):
  - Global CSS: Place in `AppEnd/assets/custom.css` or equivalent
  - Scoped overrides: Use `!important` selectors if necessary (not ideal but sometimes required for embedded third-party UI)
  - Theme variables: Define CSS custom properties for colors, sizes

### 3. Hiding Elsa Branding
- Remove/hide Elsa logo from header
- Remove "Powered by Elsa" or similar text
- Remove Elsa-specific help links (replace with AppEnd docs)
- Customize footer to show AppEnd copyright only

### 4. UI Components & Navigation
- **Workflow Designer**:
  - Embedded Elsa designer canvas
  - Activity palette (AppEnd-styled)
  - Properties panel (AppEnd-themed)
  
- **Workflow Management**:
  - AppEnd dashboard with "Workflows" section
  - List of workflow definitions (AppEnd-styled table/grid)
  - Create, edit, delete, deploy actions
  - Execute workflow from management UI

- **Execution Monitoring**:
  - AppEnd dashboard widget showing active workflows
  - Drill-down to execution history and details

### 5. Authentication & Authorization
- **SSO Integration**:
  - Leverage AppEnd's authentication (session, JWT, OAuth, etc.)
  - No separate Elsa login; inherit AppEnd user context
  - Pass user info to Elsa (for audit, permissions)

- **Permission Model**:
  - Use AppEnd roles (Admin, Developer, Operator)
  - Control access to designer, execution, history views
  - Audit: Log user actions to AppEnd audit trail

### 6. Implementation Details

#### CSS Customization File
```
AppEnd/assets/workflow-custom.css
```
Contains:
- CSS variables for Elsa Studio overrides
- Color scheme mappings
- Logo/branding image replacements
- Component styling tweaks

#### Startup Configuration
```csharp
// In AppEndServer startup (Phase 1)
services.AddElsa(...)
    .UseEntityFrameworkPersistence(...)
    .AddHttp() // for HTTP activities if needed
    .AddJavaScript(); // for scripting activities if needed

// Custom theming might require:
// - Registering custom activity descriptors with AppEnd icons
// - Providing custom CSS bundle
```

#### Layout Integration
```html
<!-- AppEnd layout wrapper for Elsa Studio -->
<div class="app-container">
  <AppEndNavbar />
  <div class="app-content">
    <!-- Elsa Studio iframe or direct embed -->
    <ElsaStudioComponent />
  </div>
  <AppEndFooter />
</div>
```

### 7. Customization Areas

| Area | Default Elsa | AppEnd Custom |
|------|--------------|---------------|
| **Logo** | Elsa logo (top-left) | AppEnd logo or text |
| **Primary Color** | Blue (#007ACC) | AppEnd brand color |
| **Typography** | Segoe UI | AppEnd font (if defined) |
| **Icons** | Elsa icon set | AppEnd icon set (if available) |
| **Help/Docs** | Elsa docs links | AppEnd workflow docs |
| **Footer** | "Elsa Workflows" | "AppEnd Workflows" or empty |
| **Dashboard** | Elsa dashboard | AppEnd dashboard integration |
| **User Menu** | Elsa user profile | AppEnd user profile (inherited) |

### 8. Limitations & Considerations

- **Elsa Version Upgrades**: CSS selectors might change; plan for regression testing
- **Deep Customization**: Some UI elements may require forking Elsa Studio components (future effort)
- **Performance**: Embedding adds layer of indirection; monitor UI responsiveness
- **Browser Compatibility**: Ensure Elsa Studio works in AppEnd's supported browsers

### 9. Testing & Validation

- **Visual Regression Tests**: Screenshot comparisons to ensure branding consistency
- **Accessibility**: WCAG compliance (inherit AppEnd standards)
- **Cross-browser**: Test on AppEnd's supported browser list
- **Mobile Responsiveness**: If AppEnd is mobile-friendly, ensure workflows designer is also responsive

### 10. Future Enhancements (Phase 4+)

- **Custom Designer UI**: Build a fully custom workflow designer from scratch (replaces Elsa Studio)
- **Advanced Branding**: Configurable themes (light/dark mode, custom palettes)
- **Localization**: Multi-language support in designer and management UI
- **Mobile Designer**: Responsive workflow designer for tablets/phones

---

## Summary

**Phase 4 Deliverable**: Elsa Studio embedded in AppEnd with:
- ✅ AppEnd color scheme and typography
- ✅ AppEnd logo (Elsa logo hidden)
- ✅ Single sign-on (no separate login)
- ✅ AppEnd navigation integration
- ✅ Custom CSS theme bundle
- ✅ Workflow management dashboard in AppEnd
