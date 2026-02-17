# Phase 1: Foundation - Implementation Guide

## Overview
Phase 1 establishes the core infrastructure for integrating Elsa 3.0 workflow engine into AppEnd. This includes:
- Service registration and dependency injection
- Entity Framework Core data persistence
- SQL Server database integration
- Core service interfaces and implementations
- Foundational abstractions for AppEnd consumers

## Components Created

### 1. **WorkflowServices.cs**
Service registration extension for Elsa integration.

**Key Features:**
- Registers Elsa with default features
- Configures Entity Framework Core with SQL Server
- Registers AppEnd-specific workflow services
- Single extension point for AppEnd's dependency injection

**Usage:**
```csharp
// In Program.cs ConfigServices method:
builder.Services.AddAppEndWorkflows(
    connectionString: "Server=...;Database=ElsaWorkflows;...",
    configuration: builder.Configuration);
```

### 2. **IWorkflowService.cs**
Main facade interface providing high-level workflow operations.

**Key Methods:**
- `ExecuteWorkflowAsync`: Run a workflow with optional input
- `ResumeWorkflowAsync`: Resume a suspended workflow
- `SuspendWorkflowAsync`: Pause a running workflow
- `CancelWorkflowAsync`: Terminate a workflow instance

**Properties:**
- `Definitions`: Access workflow definition operations
- `Instances`: Access workflow instance querying

### 3. **IWorkflowDefinitionService.cs**
Service for managing workflow definitions (CRUD and lifecycle).

**Key Methods:**
- `GetByIdAsync`: Retrieve definition by ID
- `GetByNameAsync`: Retrieve definition by name
- `ListAsync`: List all definitions (optionally filtered by tenant)
- `CreateAsync`: Create new definition
- `UpdateAsync`: Modify existing definition
- `PublishAsync`: Publish a definition version
- `DeleteAsync`: Delete a definition

**DTOs:**
- `WorkflowDefinitionDto`: Definition data
- `CreateWorkflowDefinitionRequest`: Creation payload
- `UpdateWorkflowDefinitionRequest`: Update payload

### 4. **IWorkflowInstanceService.cs**
Service for querying and monitoring workflow instances.

**Key Methods:**
- `GetByIdAsync`: Retrieve instance by ID
- `GetByCorrelationIdAsync`: Find instances by correlation ID
- `ListAsync`: List with pagination and filtering
- `GetExecutionHistoryAsync`: Get event log for instance
- `GetActivityExecutionsAsync`: Get individual activity executions

**DTOs:**
- `WorkflowInstanceDto`: Instance data
- `WorkflowInstanceFilter`: Query filter criteria
- `WorkflowInstanceEventDto`: Execution event
- `ActivityExecutionDto`: Activity execution record
- `PagedResult<T>`: Generic paged result wrapper

### 5. **WorkflowService.cs**
Main implementation of IWorkflowService.

**Features:**
- Delegates to definition and instance services
- Integrates with Elsa's `IWorkflowRuntime`
- Comprehensive logging
- Error handling with detailed messages
- Ready for Phase 2 implementation details

### 6. **WorkflowDefinitionService.cs**
Implementation of IWorkflowDefinitionService.

**Features:**
- CRUD operations for workflow definitions
- Logging for all operations
- Error handling and exception propagation
- Placeholder TODOs for Elsa integration

### 7. **WorkflowInstanceService.cs**
Implementation of IWorkflowInstanceService.

**Features:**
- Instance querying and monitoring
- Pagination support
- Execution history and activity tracking
- Structured logging

### 8. **Sample Workflow Classes**
Code-first workflow definitions demonstrating patterns.

**Classes:**
- `SimpleApprovalWorkflow`: Example workflow using Elsa activities
- `AppEndWorkflowBase`: Base class for AppEnd workflows with logging helpers

**Conventions:**
- Logging methods: `LogInfo()`, `LogWarning()`, `LogError()`
- Metadata properties: `WorkflowName`, `WorkflowDisplayName`, `Version`, `TenantId`

## Architecture Decisions

### Service Layer Pattern
All workflow consumers (UI, schedulers, events) interact through `IWorkflowService`, not directly with Elsa. This:
- Decouples AppEnd from Elsa's API changes
- Allows future replacement of workflow engine
- Centralizes workflow logic
- Enables consistent logging and error handling

### Dependency Injection
Services are registered as **scoped** (per HTTP request):
- Definition Service: Scoped
- Instance Service: Scoped
- Main Service: Scoped
- Elsa Runtime: Per-request (managed by Elsa)

### Database Design
Uses **Entity Framework Core** with SQL Server:
- Leverages Elsa's built-in EF Core persistence provider
- Uses existing AppEnd database connection or separate Elsa database
- Supports migration-based schema management
- Tables created via Elsa's schema initializer

### Multi-Tenancy
Support built into filters and queries:
- `TenantId` field in all instance DTOs
- Optional tenant filtering in list operations
- Tenant-aware workflow isolation

## Next Steps (Phase 2)

### 1. Implement Actual Workflow Operations
Replace TODO placeholders in service implementations with Elsa integration:
- Use Elsa's `IWorkflowStore` for definition persistence
- Use Elsa's `IWorkflowInstanceStore` for instance management
- Integrate with `IWorkflowRuntime` for execution

### 2. Wire into Program.cs
Add to `Program.cs`:
```csharp
// In ConfigServices:
var connStr = builder.Configuration.GetConnectionString("ElsaWorkflows") 
    ?? AppEndSettings.GetConnStr("DefaultConnection");
builder.Services.AddAppEndWorkflows(connStr, builder.Configuration);
```

### 3. Update AppEndServer.csproj
Add NuGet packages:
```xml
<PackageReference Include="Elsa" Version="3.0.x" />
<PackageReference Include="Elsa.Persistence.EntityFrameworkCore.SqlServer" Version="3.0.x" />
```

### 4. Run Database Migrations
```bash
dotnet ef migrations add "AddElsaWorkflows" -p AppEndServer
dotnet ef database update -p AppEndServer
```

### 5. Create WorkflowDbContext
Custom DbContext extending Elsa's `ElsaContext` with AppEnd-specific tables:
- Approval instances
- Workflow suspensions
- Custom audit logs
- AppEnd-specific metadata

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AppEnd;...",
    "ElsaWorkflows": "Server=localhost;Database=ElsaWorkflows;..."
  },
  "Elsa": {
    "Features": {
      "EnableWorkflowDefinitions": true,
      "EnableWorkflowInstances": true,
      "EnableApprovals": true
    }
  }
}
```

### Environment Variables
- `ELSA_DB_CONNECTION`: Override database connection
- `ELSA_FEATURES_*`: Enable/disable features

## Testing Recommendations

### Unit Tests
- Mock `IWorkflowRuntime` for service tests
- Test filter logic in instance service
- Test pagination in list operations
- Test error handling and logging

### Integration Tests
- Use test database (Elsa provides test utilities)
- Test full workflow execution lifecycle
- Verify persistence and retrieval
- Test multi-tenant isolation

### Manual Testing
1. Register services and build app
2. Verify dependency injection resolution
3. Call `ExecuteWorkflowAsync` with sample workflow
4. Query instances via `ListAsync`
5. Check logs for proper messaging

## Troubleshooting

### Common Issues

**Issue: "IWorkflowRuntime not registered"**
- Ensure `AddAppEndWorkflows()` is called in `ConfigServices`
- Verify Elsa NuGet package is installed

**Issue: "Database schema not initialized"**
- Run Entity Framework migrations
- Ensure connection string is valid
- Check SQL Server is running and accessible

**Issue: "Workflow definition not found"**
- Verify definition was created and published
- Check tenant ID matches in queries
- Ensure database transaction is committed

## Files Added

```
AppEndServer/
├── Workflows/
│   ├── IWorkflowService.cs
│   ├── IWorkflowDefinitionService.cs
│   ├── IWorkflowInstanceService.cs
│   ├── WorkflowService.cs
│   ├── WorkflowDefinitionService.cs
│   ├── WorkflowInstanceService.cs
│   ├── WorkflowServices.cs
│   ├── PROGRAM_CS_INTEGRATION_INSTRUCTIONS.txt
│   ├── Samples/
│   │   └── SimpleApprovalWorkflow.cs
│   └── README.md (this file)
```

## Summary

Phase 1 provides a solid foundation for Elsa integration:
- ✅ Service registration and DI
- ✅ Core interface contracts
- ✅ Initial implementations with logging
- ✅ Sample workflow structure
- ✅ Database abstraction via Entity Framework
- ✅ Multi-tenant support framework

Ready to proceed to **Phase 2: Integration** where we'll connect these services to AppEnd's existing infrastructure (scheduling, events, UI).
