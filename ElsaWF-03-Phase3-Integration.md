# Phase 3 — Integration with AppEndHost

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## Goal
Wire Elsa into the host application.

## Changes to `AppEndHost/AppEndHost.csproj`
Add project reference:
```xml
<ProjectReference Include="..\AppEndWorkflow\AppEndWorkflow.csproj" />
```

## Changes to `AppEndHost/Program.cs`

### In `ConfigServices` method — add:
```csharp
builder.Services.AddAppEndWorkflow();
```
After existing service registrations (CORS, compression, scheduler).

### In main app pipeline — add:
```csharp
app.UseAppEndWorkflow();
```
Before `app.UseRouting()`.

## Final Program.cs Pipeline Order
```
app.UseCors("AppEndPolicy");
app.UseResponseCompression();
app.UseForwardedHeaders(...);
app.UseHttpsRedirection();
app.UseAppEndWorkflow();          // ← NEW (Elsa internal middleware only, no API routes)
app.UseRpcNet();                  // all workflow operations go through here via rpcAEP
app.UseFileServer(...);
app.UseRouting();
app.Run();
```

> **Important:** Elsa does NOT register any HTTP routes. All workflow management
> (CRUD, execution, monitoring) is accessed exclusively through the existing
> `talk-to-me` RPC endpoint via `WorkflowServices.cs` bridge methods.
