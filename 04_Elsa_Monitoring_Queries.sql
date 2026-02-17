-- ==================================================================================
-- Elsa 3.0 - Useful Queries for Monitoring & Management
-- ==================================================================================
-- This file contains useful queries for monitoring Elsa
-- ==================================================================================

-- =================================================================================
-- 1. Workflow Status Overview - Overview of Workflow Status
-- =================================================================================

SELECT 
    [Status],
    COUNT(*) AS [Count],
    COUNT(*) * 100.0 / (SELECT COUNT(*) FROM [dbo].[ElsaWorkflowInstances]) AS [Percentage]
FROM [dbo].[ElsaWorkflowInstances]
WHERE [IsDeleted] = 0
GROUP BY [Status]
ORDER BY [Count] DESC;

-- =================================================================================
-- 2. Running Workflows - Workflows in Progress
-- =================================================================================

SELECT TOP 100
    wi.[Id],
    wd.[Name] AS [WorkflowName],
    wi.[Status],
    wi.[CreatedAt],
    wi.[StartedAt],
    DATEDIFF(HOUR, wi.[StartedAt], GETUTCDATE()) AS [RunningHours],
    wi.[UserId],
    wi.[TenantId]
FROM [dbo].[ElsaWorkflowInstances] wi
LEFT JOIN [dbo].[ElsaWorkflowDefinitions] wd ON wi.[WorkflowDefinitionId] = wd.[Id]
WHERE wi.[Status] = 'Running'
    AND wi.[IsDeleted] = 0
ORDER BY wi.[StartedAt] DESC;

-- =================================================================================
-- 3. Failed Workflows - Workflows with Faults
-- =================================================================================

SELECT TOP 50
    wi.[Id],
    wd.[Name] AS [WorkflowName],
    wi.[Status],
    wi.[FaultedAt],
    wi.[ExceptionMessage],
    wi.[UserId]
FROM [dbo].[ElsaWorkflowInstances] wi
LEFT JOIN [dbo].[ElsaWorkflowDefinitions] wd ON wi.[WorkflowDefinitionId] = wd.[Id]
WHERE wi.[Status] = 'Faulted'
    AND wi.[IsDeleted] = 0
ORDER BY wi.[FaultedAt] DESC;

-- =================================================================================
-- 4. Workflow Execution Statistics - Workflow Execution Statistics
-- =================================================================================

SELECT 
    wd.[Name] AS [WorkflowName],
    COUNT(wi.[Id]) AS [TotalInstances],
    SUM(CASE WHEN wi.[Status] = 'Running' THEN 1 ELSE 0 END) AS [Running],
    SUM(CASE WHEN wi.[Status] = 'Completed' THEN 1 ELSE 0 END) AS [Completed],
    SUM(CASE WHEN wi.[Status] = 'Faulted' THEN 1 ELSE 0 END) AS [Faulted],
    SUM(CASE WHEN wi.[Status] = 'Suspended' THEN 1 ELSE 0 END) AS [Suspended],
    AVG(DATEDIFF(MINUTE, wi.[StartedAt], ISNULL(wi.[CompletedAt], GETUTCDATE()))) AS [AvgDurationMinutes]
FROM [dbo].[ElsaWorkflowDefinitions] wd
LEFT JOIN [dbo].[ElsaWorkflowInstances] wi ON wd.[Id] = wi.[WorkflowDefinitionId]
WHERE wd.[IsDeleted] = 0
GROUP BY wd.[Name]
ORDER BY [TotalInstances] DESC;

-- =================================================================================
-- 5. Activity Performance - Activity Performance
-- =================================================================================

SELECT TOP 50
    [ActivityType],
    COUNT(*) AS [ExecutionCount],
    AVG(DATEDIFF(MILLISECOND, [StartedAt], [CompletedAt])) AS [AvgDurationMs],
    SUM(CASE WHEN [Status] = 'Completed' THEN 1 ELSE 0 END) AS [Successful],
    SUM(CASE WHEN [Status] = 'Faulted' THEN 1 ELSE 0 END) AS [Failed]
FROM [dbo].[ElsaActivityExecutions]
WHERE [StartedAt] IS NOT NULL
    AND [CompletedAt] IS NOT NULL
GROUP BY [ActivityType]
ORDER BY [ExecutionCount] DESC;

-- =================================================================================
-- 6. Pending Approvals - Pending Approvals
-- =================================================================================

SELECT TOP 100
    ap.[Id],
    ap.[Title],
    ap.[Description],
    ap.[RequestedBy],
    ap.[RequestedFor],
    ap.[Status],
    ap.[DueDate],
    DATEDIFF(DAY, GETUTCDATE(), ap.[DueDate]) AS [DaysRemaining],
    wi.[CorrelationId]
FROM [dbo].[ElsaApprovalInstances] ap
LEFT JOIN [dbo].[ElsaWorkflowInstances] wi ON ap.[WorkflowInstanceId] = wi.[Id]
WHERE ap.[Status] = 'Pending'
    AND ap.[DueDate] >= GETUTCDATE()
ORDER BY ap.[DueDate] ASC;

-- =================================================================================
-- 7. Overdue Approvals - Overdue Approvals
-- =================================================================================

SELECT
    ap.[Id],
    ap.[Title],
    ap.[RequestedFor],
    ap.[DueDate],
    DATEDIFF(DAY, ap.[DueDate], GETUTCDATE()) AS [DaysOverdue],
    ap.[ReminderSent]
FROM [dbo].[ElsaApprovalInstances] ap
WHERE ap.[Status] = 'Pending'
    AND ap.[DueDate] < GETUTCDATE()
ORDER BY [DaysOverdue] DESC;

-- =================================================================================
-- 8. Suspended Workflows - Suspended Workflows
-- =================================================================================

SELECT
    ws.[Id],
    wi.[Id] AS [WorkflowInstanceId],
    wd.[Name] AS [WorkflowName],
    ws.[Reason],
    ws.[SuspendedBy],
    ws.[SuspendedAt],
    DATEDIFF(HOUR, ws.[SuspendedAt], GETUTCDATE()) AS [SuspendedHours],
    ws.[Notes]
FROM [dbo].[ElsaWorkflowSuspensions] ws
LEFT JOIN [dbo].[ElsaWorkflowInstances] wi ON ws.[WorkflowInstanceId] = wi.[Id]
LEFT JOIN [dbo].[ElsaWorkflowDefinitions] wd ON wi.[WorkflowDefinitionId] = wd.[Id]
WHERE ws.[ResumedAt] IS NULL
ORDER BY ws.[SuspendedAt] DESC;

-- =================================================================================
-- 9. Execution Logs Summary - Execution Logs Summary
-- =================================================================================

SELECT TOP 100
    [WorkflowInstanceId],
    [EventName],
    COUNT(*) AS [Count],
    MAX([Timestamp]) AS [LastOccurrence]
FROM [dbo].[ElsaWorkflowExecutionLogs]
WHERE [Timestamp] >= DATEADD(DAY, -1, GETUTCDATE())
GROUP BY [WorkflowInstanceId], [EventName]
ORDER BY [LastOccurrence] DESC;

-- =================================================================================
-- 10. Workflow Definitions by Status - Workflow Definitions by Status
-- =================================================================================

SELECT 
    [Name],
    [DisplayName],
    [Version],
    [PublishedVersion],
    [IsPublished],
    [IsPaused],
    [DefinitionFormat],
    [CreatedAt],
    [UpdatedAt],
    [CreatedBy]
FROM [dbo].[ElsaWorkflowDefinitions]
WHERE [IsDeleted] = 0
ORDER BY [UpdatedAt] DESC;

-- =================================================================================
-- 11. Recently Modified Workflows - Recently Modified Workflows
-- =================================================================================

SELECT TOP 20
    [Name],
    [DisplayName],
    [Version],
    [UpdatedAt],
    [UpdatedBy],
    [IsPublished]
FROM [dbo].[ElsaWorkflowDefinitions]
WHERE [IsDeleted] = 0
ORDER BY [UpdatedAt] DESC;

-- =================================================================================
-- 12. Database Size Information - Database Size Information
-- =================================================================================

SELECT 
    OBJECT_NAME(i.object_id) AS [TableName],
    SUM(s.row_count) AS [RowCount],
    SUM(s.reserved_page_count * 8) / 1024 AS [SizeKB]
FROM sys.dm_db_partition_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE OBJECT_NAME(i.object_id) LIKE 'Elsa%'
    AND i.type_desc = 'HEAP' OR (i.type_desc = 'CLUSTERED INDEX')
GROUP BY OBJECT_NAME(i.object_id)
ORDER BY [SizeKB] DESC;

-- =================================================================================
-- 13. Audit Log Recent Changes - Recent Changes in Audit Log
-- =================================================================================

SELECT TOP 100
    [EntityType],
    [EntityId],
    [Action],
    [ChangedBy],
    [ChangedAt],
    [IpAddress]
FROM [dbo].[ElsaAuditLogs]
ORDER BY [ChangedAt] DESC;

-- =================================================================================
-- 14. Multi-Tenant Statistics - Multi-Tenant Statistics
-- =================================================================================

SELECT 
    ISNULL([TenantId], 'Default') AS [TenantId],
    COUNT(DISTINCT [Id]) AS [WorkflowCount],
    SUM(CASE WHEN [Status] = 'Running' THEN 1 ELSE 0 END) AS [RunningCount],
    SUM(CASE WHEN [Status] = 'Completed' THEN 1 ELSE 0 END) AS [CompletedCount]
FROM [dbo].[ElsaWorkflowInstances]
WHERE [IsDeleted] = 0
GROUP BY [TenantId]
ORDER BY [WorkflowCount] DESC;

-- =================================================================================
-- 15. Variables Used in Workflows - Variables Used in Workflows
-- =================================================================================

SELECT TOP 100
    [Name],
    [Type],
    COUNT(DISTINCT [WorkflowInstanceId]) AS [UsageCount],
    SUM(CASE WHEN [IsVolatile] = 1 THEN 1 ELSE 0 END) AS [VolatileCount]
FROM [dbo].[ElsaVariableInstances]
GROUP BY [Name], [Type]
ORDER BY [UsageCount] DESC;

-- =================================================================================
-- 16. Bookmarks Status - Bookmarks Status
-- =================================================================================

SELECT
    SUM(CASE WHEN [IsProcessed] = 0 THEN 1 ELSE 0 END) AS [PendingBookmarks],
    SUM(CASE WHEN [IsProcessed] = 1 THEN 1 ELSE 0 END) AS [ProcessedBookmarks],
    COUNT(*) AS [TotalBookmarks]
FROM [dbo].[ElsaBookmarks];

-- =================================================================================
-- 17. Clean Old Data - Clean Old Data
-- =================================================================================

-- WARNING: This query deletes old records (e.g., 90 days ago)
-- Only run if backup is in place!

/*
DELETE FROM [dbo].[ElsaWorkflowInstances]
WHERE [IsDeleted] = 1
    AND [DeletedAt] < DATEADD(DAY, -90, GETUTCDATE());

DELETE FROM [dbo].[ElsaWorkflowExecutionLogs]
WHERE [Timestamp] < DATEADD(DAY, -180, GETUTCDATE());
*/

-- =================================================================================
-- 18. Create Indexes If Missing - Create Indexes If Missing
-- =================================================================================

-- Index for faster search
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ElsaWorkflowInstances_CorrelationId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ElsaWorkflowInstances_CorrelationId]
    ON [dbo].[ElsaWorkflowInstances]([CorrelationId]);
END

-- =================================================================================
-- Important Notes:
-- =================================================================================
-- 1. Use these queries to monitor system health
-- 2. Review results regularly
-- 3. For Production, create a scheduled job to clean old data
-- 4. Ensure backup before making any data changes
