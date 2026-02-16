# Phase 1 — Create AppEndWorkflow Project

> Part of [Elsa Integration Plan](ELSA-INTEGRATION-PLAN.md)

---

## Goal
Create a new Class Library project that encapsulates all Elsa-related logic.

## Steps

1. **Create project:**
   ```
   AppEndWorkflow/AppEndWorkflow.csproj  (net10.0, Class Library)
   ```

2. **Add NuGet packages:**
   - `Elsa` (3.5.3) — Core workflow engine, runtime, activities, HTTP module
   - `Elsa.EntityFrameworkCore.SqlServer` — SQL Server persistence provider

3. **Add project references:**
   - `AppEndCommon` — for `AppEndSettings`, logging
   - `AppEndDbIO` — for `DbConf.FromSettings()` to read connection string

4. **Add to Solution:**
   - Register in `AppEnd.sln`

## Expected csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Elsa" Version="3.5.3" />
    <PackageReference Include="Elsa.EntityFrameworkCore.SqlServer" Version="3.5.3" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\AppEndCommon\AppEndCommon.csproj" />
    <ProjectReference Include="..\AppEndDbIO\AppEndDbIO.csproj" />
  </ItemGroup>
</Project>
```
