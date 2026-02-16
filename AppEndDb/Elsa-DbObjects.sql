-- ================================================================================
-- ELSA PACKAGE CONSOLIDATED FILE
-- This file contains all SQL objects from the Elsa package
-- Created: February 16, 2026
-- Status: TEMPORARY - Ready for deletion after backup
-- Updated: All object names now match file names
-- ================================================================================

-- =============================================
-- FILE: ElsaWorkflowDefinitions.sql
-- =============================================
-- =============================================
-- Name:        ElsaWorkflowDefinitions
-- Description: Elsa 3.5.3 WorkflowDefinitions table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaWorkflowDefinitions')
BEGIN
    CREATE TABLE [dbo].[ElsaWorkflowDefinitions]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(255) NULL,
        [Description] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIMEOFFSET NOT NULL,
        [UpdatedAt] DATETIMEOFFSET NULL,
        [PublishedAt] DATETIMEOFFSET NULL,
        [Version] INT NOT NULL,
        [IsLatest] BIT NOT NULL,
        [IsPublished] BIT NOT NULL,
        [TenantId] NVARCHAR(450) NULL,
        [Data] NVARCHAR(MAX) NULL,
        [HashValue] NVARCHAR(MAX) NULL,
        [IsReadonly] BIT NOT NULL,
        [ToolVersion] INT NULL,
        [DefinitionVersionId] NVARCHAR(450) NOT NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaWorkflowInstances.sql
-- =============================================
-- =============================================
-- Name:        ElsaWorkflowInstances
-- Description: Elsa 3.5.3 WorkflowInstances table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaWorkflowInstances')
BEGIN
    CREATE TABLE [dbo].[ElsaWorkflowInstances]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [DefinitionId] NVARCHAR(450) NOT NULL,
        [DefinitionVersionId] NVARCHAR(450) NOT NULL,
        [TenantId] NVARCHAR(450) NULL,
        [ParentId] NVARCHAR(450) NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [SubStatus] NVARCHAR(50) NULL,
        [CorrelationId] NVARCHAR(450) NULL,
        [CreatedAt] DATETIMEOFFSET NOT NULL,
        [StartedAt] DATETIMEOFFSET NULL,
        [FinishedAt] DATETIMEOFFSET NULL,
        [UpdatedAt] DATETIMEOFFSET NULL,
        [CancelledAt] DATETIMEOFFSET NULL,
        [SerializedProperties] NVARCHAR(MAX) NULL,
        [SerializedData] NVARCHAR(MAX) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaActivityInstances.sql
-- =============================================
-- =============================================
-- Name:        ElsaActivityInstances
-- Description: Elsa 3.5.3 ActivityInstances table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaActivityInstances')
BEGIN
    CREATE TABLE [dbo].[ElsaActivityInstances]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [InstanceId] NVARCHAR(450) NOT NULL,
        [ActivityId] NVARCHAR(450) NOT NULL,
        [ActivityType] NVARCHAR(450) NOT NULL,
        [ActivityName] NVARCHAR(450) NULL,
        [StartedAt] DATETIMEOFFSET NULL,
        [FinishedAt] DATETIMEOFFSET NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [SerializedProperties] NVARCHAR(MAX) NULL,
        [SerializedData] NVARCHAR(MAX) NULL,
        [ExecutionLogId] NVARCHAR(450) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaBookmarks.sql
-- =============================================
-- =============================================
-- Name:        ElsaBookmarks
-- Description: Elsa 3.5.3 Bookmarks table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaBookmarks')
BEGIN
    CREATE TABLE [dbo].[ElsaBookmarks]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [InstanceId] NVARCHAR(450) NOT NULL,
        [ActivityInstanceId] NVARCHAR(450) NULL,
        [ActivityType] NVARCHAR(450) NOT NULL,
        [Hash] NVARCHAR(450) NOT NULL,
        [Metadata] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIMEOFFSET NOT NULL,
        [Data] NVARCHAR(MAX) NULL,
        [TenantId] NVARCHAR(450) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaWorkflowExecutionLogRecords.sql
-- =============================================
-- =============================================
-- Name:        ElsaWorkflowExecutionLogRecords
-- Description: Elsa 3.5.3 WorkflowExecutionLogRecords table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaWorkflowExecutionLogRecords')
BEGIN
    CREATE TABLE [dbo].[ElsaWorkflowExecutionLogRecords]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [InstanceId] NVARCHAR(450) NOT NULL,
        [ActivityInstanceId] NVARCHAR(450) NULL,
        [Timestamp] DATETIMEOFFSET NOT NULL,
        [EventName] NVARCHAR(450) NOT NULL,
        [Message] NVARCHAR(MAX) NULL,
        [LogLevel] NVARCHAR(50) NULL,
        [SerializedData] NVARCHAR(MAX) NULL,
        [TenantId] NVARCHAR(450) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaLabels.sql
-- =============================================
-- =============================================
-- Name:        ElsaLabels
-- Description: Elsa 3.5.3 Labels table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaLabels')
BEGIN
    CREATE TABLE [dbo].[ElsaLabels]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(450) NOT NULL,
        [TenantId] NVARCHAR(450) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaWorkflowDefinitionLabels.sql
-- =============================================
-- =============================================
-- Name:        ElsaWorkflowDefinitionLabels
-- Description: Elsa 3.5.3 WorkflowDefinitionLabels table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaWorkflowDefinitionLabels')
BEGIN
    CREATE TABLE [dbo].[ElsaWorkflowDefinitionLabels]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [DefinitionId] NVARCHAR(450) NOT NULL,
        [LabelId] NVARCHAR(450) NOT NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaStoredBookmarks.sql
-- =============================================
-- =============================================
-- Name:        ElsaStoredBookmarks
-- Description: Elsa 3.5.3 StoredBookmarks table for SQL Server (for persistence of trigger data)
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaStoredBookmarks')
BEGIN
    CREATE TABLE [dbo].[ElsaStoredBookmarks]
    (
        [Id] NVARCHAR(450) NOT NULL,
        [BookmarkId] NVARCHAR(450) NOT NULL,
        [ActivityType] NVARCHAR(450) NOT NULL,
        [SerializedData] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIMEOFFSET NOT NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END

-- =============================================
-- FILE: ElsaWorkflowTasks.sql
-- =============================================
-- =============================================
-- Name:        ElsaWorkflowTasks
-- Description: Elsa 3.5.3 WorkflowTasks table for SQL Server
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ElsaWorkflowTasks')
BEGIN
    CREATE TABLE [dbo].[ElsaWorkflowTasks]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [InstanceId] NVARCHAR(450) NOT NULL,
        [DefinitionId] NVARCHAR(450) NOT NULL,
        [BookmarkId] NVARCHAR(450) NULL,
        [Title] NVARCHAR(500) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [AssignedTo] NVARCHAR(100) NULL,
        [AssignedRole] NVARCHAR(100) NULL,
        [Priority] NVARCHAR(50) NOT NULL DEFAULT 'Normal',
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [DueDate] DATETIMEOFFSET NULL,
        [CreatedAt] DATETIMEOFFSET NOT NULL,
        [CompletedAt] DATETIMEOFFSET NULL,
        [CompletedBy] NVARCHAR(100) NULL,
        [Outcome] NVARCHAR(100) NULL,
        [Comment] NVARCHAR(MAX) NULL,
        [ContextData] NVARCHAR(MAX) NULL,
        [CreatedBy] NVARCHAR(100) NULL,
        [ModifiedAt] DATETIMEOFFSET NULL,
        [ModifiedBy] NVARCHAR(100) NULL,
        [TenantId] NVARCHAR(450) NULL
    )
END

-- =============================================
-- FILE: ElsaMyPendingTasks.sql
-- =============================================
-- =============================================
-- Name:        ElsaMyPendingTasks
-- Description: Elsa 3.5.3 View for pending workflow tasks
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[ElsaMyPendingTasks]', N'V') IS NOT NULL
    DROP VIEW [dbo].[ElsaMyPendingTasks];
GO

CREATE VIEW [dbo].[ElsaMyPendingTasks]
AS
SELECT 
    t.[Id],
    t.[InstanceId],
    t.[DefinitionId],
    t.[Title],
    t.[Description],
    t.[Priority],
    t.[DueDate],
    t.[CreatedAt],
    t.[AssignedTo],
    t.[AssignedRole],
    t.[ContextData],
    CASE 
        WHEN t.[DueDate] IS NOT NULL AND t.[DueDate] < GETUTCDATE() 
        THEN 1 
        ELSE 0 
    END AS [IsOverdue],
    DATEDIFF(DAY, GETUTCDATE(), t.[DueDate]) AS [DaysRemaining]
FROM [dbo].[ElsaWorkflowTasks] t
WHERE t.[Status] = 'Pending' AND t.[TenantId] IS NOT NULL;
GO

PRINT 'View ElsaMyPendingTasks created successfully!';
GO

-- =============================================
-- FILE: ElsaWorkflowTaskStats.sql
-- =============================================
-- =============================================
-- Name:        ElsaWorkflowTaskStats
-- Description: Elsa 3.5.3 View for workflow task statistics
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[ElsaWorkflowTaskStats]', N'V') IS NOT NULL
    DROP VIEW [dbo].[ElsaWorkflowTaskStats];
GO

CREATE VIEW [dbo].[ElsaWorkflowTaskStats]
AS
SELECT 
    [DefinitionId],
    COUNT(*) AS [TotalTasks],
    SUM(CASE WHEN [Status] = 'Pending' THEN 1 ELSE 0 END) AS [PendingCount],
    SUM(CASE WHEN [Status] = 'Completed' THEN 1 ELSE 0 END) AS [CompletedCount],
    SUM(CASE WHEN [Status] = 'Cancelled' THEN 1 ELSE 0 END) AS [CancelledCount],
    SUM(CASE WHEN [DueDate] < GETUTCDATE() AND [Status] = 'Pending' THEN 1 ELSE 0 END) AS [OverdueCount],
    AVG(CASE WHEN [CompletedAt] IS NOT NULL THEN DATEDIFF(HOUR, [CreatedAt], [CompletedAt]) ELSE NULL END) AS [AvgCompletionTimeHours]
FROM [dbo].[ElsaWorkflowTasks]
GROUP BY [DefinitionId];
GO

PRINT 'View ElsaWorkflowTaskStats created successfully!';
GO

-- =============================================
-- FILE: ElsaCompleteWorkflowTask.sql
-- =============================================
-- =============================================
-- Name:        ElsaCompleteWorkflowTask
-- Description: Elsa 3.5.3 Stored Procedure to complete a workflow task
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[ElsaCompleteWorkflowTask]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[ElsaCompleteWorkflowTask];
GO

CREATE PROCEDURE [dbo].[ElsaCompleteWorkflowTask]
    @TaskId UNIQUEIDENTIFIER,
    @UserId NVARCHAR(100),
    @Outcome NVARCHAR(100),
    @Comment NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate task exists and is pending
    IF NOT EXISTS (SELECT 1 FROM [dbo].[ElsaWorkflowTasks] WHERE [Id] = @TaskId AND [Status] = 'Pending')
    BEGIN
        RAISERROR('Task not found or already completed', 16, 1);
        RETURN;
    END

    -- Update task
    UPDATE [dbo].[ElsaWorkflowTasks]
    SET 
        [Status] = 'Completed',
        [CompletedAt] = GETUTCDATE(),
        [CompletedBy] = @UserId,
        [Outcome] = @Outcome,
        [Comment] = @Comment,
        [ModifiedAt] = GETUTCDATE(),
        [ModifiedBy] = @UserId
    WHERE [Id] = @TaskId;

    -- Return updated task
    SELECT 
        [Id],
        [InstanceId],
        [BookmarkId],
        [Outcome],
        [CompletedAt]
    FROM [dbo].[ElsaWorkflowTasks]
    WHERE [Id] = @TaskId;
END;
GO

PRINT 'Stored procedure ElsaCompleteWorkflowTask created successfully!';
GO

-- =============================================
-- FILE: ElsaGetMyWorkflowTasks.sql
-- =============================================
-- =============================================
-- Name:        ElsaGetMyWorkflowTasks
-- Description: Elsa 3.5.3 Stored Procedure to retrieve user workflow tasks
-- Generated for AppEnd integration
-- =============================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[dbo].[ElsaGetMyWorkflowTasks]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[ElsaGetMyWorkflowTasks];
GO

CREATE PROCEDURE [dbo].[ElsaGetMyWorkflowTasks]
    @UserId NVARCHAR(100),
    @Status NVARCHAR(50) = NULL,
    @Page INT = 1,
    @PageSize INT = 25
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT 
        t.[Id],
        t.[InstanceId],
        t.[DefinitionId],
        t.[Title],
        t.[Description],
        t.[Priority],
        t.[Status],
        t.[DueDate],
        t.[CreatedAt],
        t.[AssignedTo],
        t.[AssignedRole],
        t.[ContextData],
        CASE 
            WHEN t.[DueDate] IS NOT NULL AND t.[DueDate] < GETUTCDATE() AND t.[Status] = 'Pending'
            THEN 1 
            ELSE 0 
        END AS [IsOverdue]
    FROM [dbo].[ElsaWorkflowTasks] t
    WHERE 
        (t.[AssignedTo] = @UserId OR t.[AssignedTo] IS NULL)
        AND (@Status IS NULL OR t.[Status] = @Status)
    ORDER BY 
        t.[Priority] DESC,
        t.[CreatedAt] DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Return total count
    SELECT COUNT(*) AS [TotalCount]
    FROM [dbo].[ElsaWorkflowTasks] t
    WHERE 
        (t.[AssignedTo] = @UserId OR t.[AssignedTo] IS NULL)
        AND (@Status IS NULL OR t.[Status] = @Status);
END;
GO

PRINT 'Stored procedure ElsaGetMyWorkflowTasks created successfully!';
GO

-- =============================================
-- FILE: ElsaIndexes.sql
-- =============================================
-- =============================================
-- Name:        ElsaIndexes
-- Description: Elsa 3.5.3 indexes for better query performance
-- Generated for AppEnd integration
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowDefinitions_DefinitionId' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowDefinitions]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowDefinitions_DefinitionId ON [dbo].[ElsaWorkflowDefinitions]([DefinitionVersionId])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowInstances_DefinitionId' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowInstances]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowInstances_DefinitionId ON [dbo].[ElsaWorkflowInstances]([DefinitionId])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowInstances_DefinitionVersionId' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowInstances]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowInstances_DefinitionVersionId ON [dbo].[ElsaWorkflowInstances]([DefinitionVersionId])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaBookmarks_InstanceId' AND object_id = OBJECT_ID(N'[dbo].[ElsaBookmarks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaBookmarks_InstanceId ON [dbo].[ElsaBookmarks]([InstanceId])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaBookmarks_Hash' AND object_id = OBJECT_ID(N'[dbo].[ElsaBookmarks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaBookmarks_Hash ON [dbo].[ElsaBookmarks]([Hash])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowExecutionLogRecords_InstanceId' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowExecutionLogRecords]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowExecutionLogRecords_InstanceId ON [dbo].[ElsaWorkflowExecutionLogRecords]([InstanceId])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowTasks_Status' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowTasks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowTasks_Status ON [dbo].[ElsaWorkflowTasks]([Status])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowTasks_AssignedTo' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowTasks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowTasks_AssignedTo ON [dbo].[ElsaWorkflowTasks]([AssignedTo])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowTasks_AssignedRole' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowTasks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowTasks_AssignedRole ON [dbo].[ElsaWorkflowTasks]([AssignedRole])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowTasks_InstanceId' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowTasks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowTasks_InstanceId ON [dbo].[ElsaWorkflowTasks]([InstanceId])
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowTasks_DueDate' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowTasks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowTasks_DueDate ON [dbo].[ElsaWorkflowTasks]([DueDate]) WHERE [DueDate] IS NOT NULL
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ElsaWorkflowTasks_CreatedAt' AND object_id = OBJECT_ID(N'[dbo].[ElsaWorkflowTasks]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ElsaWorkflowTasks_CreatedAt ON [dbo].[ElsaWorkflowTasks]([CreatedAt])
END

PRINT 'Elsa 3.5.3 schema created successfully in [dbo] with Elsa prefix.'

-- ================================================================================
-- END OF CONSOLIDATED FILE
-- ================================================================================
