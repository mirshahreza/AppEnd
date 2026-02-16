#!/usr/bin/env pwsh
# Workflow Engine - Quick API Test Script
# 
# استفاده: 
# .\test-workflow-api.ps1
#
# یا SQL میں ابتدا تسک درج کنید، سپس اسکریپت اجرا کنید

param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$TaskId = ""
)

$ErrorActionPreference = "Stop"

Write-Host "🧪 Workflow Engine - API Test" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Helper function for RPC calls
function Invoke-RPC {
    param(
        [string]$Method,
        [PSCustomObject]$Params
    )
    
    $body = @{
        "jsonrpc" = "2.0"
        "id" = 1
        "method" = $Method
        "params" = $Params
    } | ConvertTo-Json -Depth 10

    Write-Host "📤 Calling: $Method" -ForegroundColor Gray
    Write-Host "📨 Params: $($Params | ConvertTo-Json -Depth 5)" -ForegroundColor Gray
    
    try {
        $response = Invoke-WebRequest -Uri "$BaseUrl/talk-to-me" `
            -Method POST `
            -ContentType "application/json" `
            -Body $body `
            -ErrorAction Stop
        
        $result = $response.Content | ConvertFrom-Json
        
        if ($result.result) {
            Write-Host "✅ Response:" -ForegroundColor Green
            Write-Host ($result.result | ConvertTo-Json -Depth 10) -ForegroundColor Green
            return $result.result
        } else {
            Write-Host "❌ Error:" -ForegroundColor Red
            Write-Host ($result.error | ConvertTo-Json) -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "❌ Exception: $_" -ForegroundColor Red
        return $null
    }
}

# Step 1: Get Workflow Definitions
Write-Host "Step 1: Getting Workflow Definitions" -ForegroundColor Yellow
Write-Host "---" -ForegroundColor Yellow
$definitions = Invoke-RPC "Zzz.AppEndProxy.GetWorkflowDefinitions" @{}
Write-Host ""

if ($definitions) {
    Write-Host "📋 Available Workflows:" -ForegroundColor Cyan
    $definitions | ForEach-Object {
        Write-Host "  • $($_.Id) - $($_.Name)" -ForegroundColor Cyan
    }
    Write-Host ""
}

# Step 2: Get Workflow Tasks
Write-Host "Step 2: Getting My Workflow Tasks" -ForegroundColor Yellow
Write-Host "---" -ForegroundColor Yellow
$tasks = Invoke-RPC "Zzz.AppEndProxy.GetMyWorkflowTasks" @{
    Status = "Pending"
    Page = 1
    PageSize = 25
}
Write-Host ""

if ($tasks -and $tasks.success -eq $true) {
    Write-Host "📊 Total Tasks: $($tasks.totalCount)" -ForegroundColor Cyan
    Write-Host "📋 Tasks:" -ForegroundColor Cyan
    $tasks.tasks | ForEach-Object {
        Write-Host "  • [$($_.TaskId)] $($_.Title)" -ForegroundColor Cyan
        Write-Host "    Status: $($_.Status) | Priority: $($_.Priority)" -ForegroundColor Cyan
        Write-Host "    Assigned to: $($_.AssignedTo)" -ForegroundColor Cyan
    }
    Write-Host ""
    
    # If no tasks exist, show info about testing
    if ($tasks.tasks.Count -eq 0) {
        Write-Host "⚠️  No pending tasks found." -ForegroundColor Yellow
        Write-Host "📝 To test, insert a task in SQL Server:" -ForegroundColor Yellow
        Write-Host ""
        Write-Host @"
USE AppEnd
GO

INSERT INTO [dbo].[WorkflowTasks] 
(
    [WorkflowInstanceId], 
    [WorkflowDefinitionId], 
    [Title], 
    [Description],
    [AssignedTo],
    [Priority],
    [Status],
    [DueDate],
    [CreatedAt],
    [CreatedBy],
    [ContextData]
)
VALUES 
(
    NEWID(), 
    'order-approval', 
    'تایید سفارش #12345',
    'سفارشی به مبلغ ۲۵ میلیون تومان',
    'admin',
    'High',
    'Pending',
    DATEADD(DAY, 3, GETUTCDATE()),
    GETUTCDATE(),
    'system',
    '{"orderId": 12345, "amount": 25000000}'
)
GO

SELECT TaskId, Title, Status FROM [dbo].[WorkflowTasks] ORDER BY CreatedAt DESC
"@ -ForegroundColor Gray
        Write-Host ""
        exit 0
    }
}
else {
    Write-Host "❌ Failed to get tasks" -ForegroundColor Red
    Write-Host ""
    exit 1
}

# Step 3: Complete First Pending Task
Write-Host "Step 3: Completing First Pending Task" -ForegroundColor Yellow
Write-Host "---" -ForegroundColor Yellow

$firstTask = $tasks.tasks[0]
if ($firstTask) {
    $useTaskId = if ([string]::IsNullOrEmpty($TaskId)) { $firstTask.TaskId } else { $TaskId }
    
    Write-Host "📝 Task to complete:" -ForegroundColor Cyan
    Write-Host "  • ID: $useTaskId" -ForegroundColor Cyan
    Write-Host "  • Title: $($firstTask.Title)" -ForegroundColor Cyan
    Write-Host ""
    
    $result = Invoke-RPC "Zzz.AppEndProxy.CompleteWorkflowTask" @{
        TaskId = $useTaskId
        Outcome = "Approve"
        OutputParams = @{
            comment = "تایید شد - خوب است"
        }
    }
    Write-Host ""
    
    if ($result -and $result.success -eq $true) {
        Write-Host "✅ Task completed successfully!" -ForegroundColor Green
        Write-Host "  • Completed by: $($result.completedBy)" -ForegroundColor Green
        Write-Host "  • Completed at: $($result.completedAt)" -ForegroundColor Green
        Write-Host ""
    }
}

# Step 4: Get Updated Tasks
Write-Host "Step 4: Getting Updated Tasks" -ForegroundColor Yellow
Write-Host "---" -ForegroundColor Yellow
$updatedTasks = Invoke-RPC "Zzz.AppEndProxy.GetMyWorkflowTasks" @{
    Status = ""
    Page = 1
    PageSize = 25
}
Write-Host ""

if ($updatedTasks -and $updatedTasks.success -eq $true) {
    Write-Host "📊 Updated Total: $($updatedTasks.totalCount)" -ForegroundColor Cyan
    Write-Host "✅ Pending: $($updatedTasks.tasks | Where-Object { $_.Status -eq 'Pending' } | Measure-Object | Select-Object -ExpandProperty Count)" -ForegroundColor Green
    Write-Host "✔️ Completed: $($updatedTasks.tasks | Where-Object { $_.Status -eq 'Completed' } | Measure-Object | Select-Object -ExpandProperty Count)" -ForegroundColor Green
    Write-Host ""
}

Write-Host "🎉 All tests completed!" -ForegroundColor Green
