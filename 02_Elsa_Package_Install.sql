-- ==================================================================================
-- Elsa 3.0 Package - Installation Script
-- ==================================================================================
-- این اسکریپت برای نصب Elsa بهعنوان Package AppEnd استفاده می‌شود
-- ==================================================================================

-- Install جداول
EXEC sp_executesql N'
    -- ==================================================================================
    -- 1. WORKFLOW DEFINITIONS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaWorkflowDefinitions]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [DisplayName] NVARCHAR(500),
        [Description] NVARCHAR(MAX),
        [Version] INT NOT NULL DEFAULT 1,
        [PublishedVersion] INT,
        [IsPublished] BIT NOT NULL DEFAULT 0,
        [IsPaused] BIT NOT NULL DEFAULT 0,
        [DefinitionFormat] NVARCHAR(50) NOT NULL DEFAULT ''Json'',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(255),
        [UpdatedBy] NVARCHAR(255),
        [TenantId] NVARCHAR(255),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [DeletedAt] DATETIME2,
        INDEX [IX_WorkflowDefinitions_Name] NONCLUSTERED ([Name]),
        INDEX [IX_WorkflowDefinitions_TenantId] NONCLUSTERED ([TenantId]),
        INDEX [IX_WorkflowDefinitions_IsPublished] NONCLUSTERED ([IsPublished]),
        INDEX [IX_WorkflowDefinitions_CreatedAt] NONCLUSTERED ([CreatedAt])
    );
    
    -- ==================================================================================
    -- 2. WORKFLOW DEFINITION VERSIONS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaWorkflowDefinitionVersions]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowDefinitionId] NVARCHAR(255) NOT NULL,
        [Version] INT NOT NULL,
        [Data] NVARCHAR(MAX) NOT NULL,
        [IsPublished] BIT NOT NULL DEFAULT 0,
        [PublishedAt] DATETIME2,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] NVARCHAR(255),
        [CustomAttributes] NVARCHAR(MAX),
        [Hash] NVARCHAR(255),
        FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [dbo].[ElsaWorkflowDefinitions]([Id]) ON DELETE CASCADE,
        INDEX [IX_WorkflowDefinitionVersions_WorkflowDefinitionId] NONCLUSTERED ([WorkflowDefinitionId]),
        INDEX [IX_WorkflowDefinitionVersions_Version] NONCLUSTERED ([WorkflowDefinitionId], [Version])
    );

    -- ==================================================================================
    -- 3. WORKFLOW INSTANCES
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaWorkflowInstances]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowDefinitionId] NVARCHAR(255) NOT NULL,
        [WorkflowDefinitionVersionId] NVARCHAR(255),
        [CorrelationId] NVARCHAR(255),
        [Status] NVARCHAR(50) NOT NULL DEFAULT ''Running'',
        [SubStatus] NVARCHAR(50),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [StartedAt] DATETIME2,
        [ResumedAt] DATETIME2,
        [CompletedAt] DATETIME2,
        [FaultedAt] DATETIME2,
        [CancelledAt] DATETIME2,
        [ExceptionMessage] NVARCHAR(MAX),
        [Variables] NVARCHAR(MAX),
        [CustomAttributes] NVARCHAR(MAX),
        [TenantId] NVARCHAR(255),
        [UserId] NVARCHAR(255),
        [Input] NVARCHAR(MAX),
        [Output] NVARCHAR(MAX),
        [Fault] NVARCHAR(MAX),
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [DeletedAt] DATETIME2,
        FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [dbo].[ElsaWorkflowDefinitions]([Id]) ON DELETE CASCADE,
        INDEX [IX_WorkflowInstances_WorkflowDefinitionId] NONCLUSTERED ([WorkflowDefinitionId]),
        INDEX [IX_WorkflowInstances_Status] NONCLUSTERED ([Status]),
        INDEX [IX_WorkflowInstances_CorrelationId] NONCLUSTERED ([CorrelationId]),
        INDEX [IX_WorkflowInstances_CreatedAt] NONCLUSTERED ([CreatedAt]),
        INDEX [IX_WorkflowInstances_TenantId] NONCLUSTERED ([TenantId])
    );

    -- ==================================================================================
    -- 4. ACTIVITY EXECUTIONS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaActivityExecutions]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [ActivityId] NVARCHAR(255) NOT NULL,
        [ActivityType] NVARCHAR(255) NOT NULL,
        [DisplayName] NVARCHAR(500),
        [Status] NVARCHAR(50) NOT NULL DEFAULT ''Pending'',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [StartedAt] DATETIME2,
        [CompletedAt] DATETIME2,
        [FaultedAt] DATETIME2,
        [Outputs] NVARCHAR(MAX),
        [ExceptionMessage] NVARCHAR(MAX),
        [ExecutionLog] NVARCHAR(MAX),
        [Sequence] INT,
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_ActivityExecutions_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId]),
        INDEX [IX_ActivityExecutions_Status] NONCLUSTERED ([Status]),
        INDEX [IX_ActivityExecutions_CreatedAt] NONCLUSTERED ([CreatedAt])
    );

    -- ==================================================================================
    -- 5. BOOKMARKS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaBookmarks]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [ActivityId] NVARCHAR(255) NOT NULL,
        [ActivityType] NVARCHAR(255) NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [Hash] NVARCHAR(255),
        [Payload] NVARCHAR(MAX),
        [Metadata] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ResumableFrom] DATETIME2,
        [IsProcessed] BIT NOT NULL DEFAULT 0,
        [ProcessedAt] DATETIME2,
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_Bookmarks_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId]),
        INDEX [IX_Bookmarks_ActivityId] NONCLUSTERED ([ActivityId]),
        INDEX [IX_Bookmarks_Hash] NONCLUSTERED ([Hash]),
        INDEX [IX_Bookmarks_IsProcessed] NONCLUSTERED ([IsProcessed])
    );

    -- ==================================================================================
    -- 6. WORKFLOW EXECUTION LOGS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaWorkflowExecutionLogs]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [ActivityExecutionId] NVARCHAR(255),
        [EventName] NVARCHAR(255) NOT NULL,
        [Message] NVARCHAR(MAX),
        [Level] NVARCHAR(50) NOT NULL DEFAULT ''Information'',
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [Data] NVARCHAR(MAX),
        [UserId] NVARCHAR(255),
        [CorrelationId] NVARCHAR(255),
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_WorkflowExecutionLogs_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId]),
        INDEX [IX_WorkflowExecutionLogs_EventName] NONCLUSTERED ([EventName]),
        INDEX [IX_WorkflowExecutionLogs_Timestamp] NONCLUSTERED ([Timestamp]),
        INDEX [IX_WorkflowExecutionLogs_Level] NONCLUSTERED ([Level])
    );

    -- ==================================================================================
    -- 7. VARIABLE INSTANCES
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaVariableInstances]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [Value] NVARCHAR(MAX),
        [Type] NVARCHAR(255),
        [IsVolatile] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_VariableInstances_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId]),
        INDEX [IX_VariableInstances_Name] NONCLUSTERED ([WorkflowInstanceId], [Name])
    );

    -- ==================================================================================
    -- 8. TRIGGERED WORKFLOWS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaTriggeredWorkflows]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowDefinitionId] NVARCHAR(255) NOT NULL,
        [WorkflowDefinitionVersionId] NVARCHAR(255),
        [TriggerType] NVARCHAR(255) NOT NULL,
        [TriggerName] NVARCHAR(255),
        [IsActive] BIT NOT NULL DEFAULT 1,
        [Payload] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [dbo].[ElsaWorkflowDefinitions]([Id]) ON DELETE CASCADE,
        INDEX [IX_TriggeredWorkflows_WorkflowDefinitionId] NONCLUSTERED ([WorkflowDefinitionId]),
        INDEX [IX_TriggeredWorkflows_TriggerType] NONCLUSTERED ([TriggerType]),
        INDEX [IX_TriggeredWorkflows_IsActive] NONCLUSTERED ([IsActive])
    );

    -- ==================================================================================
    -- 9. WORKFLOW EVENTS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaWorkflowEvents]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [EventName] NVARCHAR(255) NOT NULL,
        [Source] NVARCHAR(255),
        [Payload] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ProcessedAt] DATETIME2,
        [IsProcessed] BIT NOT NULL DEFAULT 0,
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_WorkflowEvents_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId]),
        INDEX [IX_WorkflowEvents_EventName] NONCLUSTERED ([EventName]),
        INDEX [IX_WorkflowEvents_IsProcessed] NONCLUSTERED ([IsProcessed])
    );

    -- ==================================================================================
    -- 10. WORKFLOW TRIGGERS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaWorkflowTriggers]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowDefinitionId] NVARCHAR(255) NOT NULL,
        [ActivityId] NVARCHAR(255) NOT NULL,
        [ActivityType] NVARCHAR(255) NOT NULL,
        [Name] NVARCHAR(255),
        [Hash] NVARCHAR(255),
        [Payload] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [dbo].[ElsaWorkflowDefinitions]([Id]) ON DELETE CASCADE,
        INDEX [IX_WorkflowTriggers_WorkflowDefinitionId] NONCLUSTERED ([WorkflowDefinitionId]),
        INDEX [IX_WorkflowTriggers_ActivityType] NONCLUSTERED ([ActivityType])
    );

    -- ==================================================================================
    -- 11. EXECUTION CONTEXTS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaExecutionContexts]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [ActivityExecutionId] NVARCHAR(255),
        [ContextType] NVARCHAR(255) NOT NULL,
        [Data] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ExpiresAt] DATETIME2,
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_ExecutionContexts_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId])
    );

    -- ==================================================================================
    -- 12. APPROVAL INSTANCES
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaApprovalInstances]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [ActivityExecutionId] NVARCHAR(255) NOT NULL,
        [RequestedBy] NVARCHAR(255) NOT NULL,
        [RequestedFor] NVARCHAR(255) NOT NULL,
        [Title] NVARCHAR(500),
        [Description] NVARCHAR(MAX),
        [Status] NVARCHAR(50) NOT NULL DEFAULT ''Pending'',
        [ApprovedBy] NVARCHAR(255),
        [ApprovedAt] DATETIME2,
        [RejectionReason] NVARCHAR(MAX),
        [DueDate] DATETIME2,
        [ReminderSent] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_ApprovalInstances_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId]),
        INDEX [IX_ApprovalInstances_Status] NONCLUSTERED ([Status]),
        INDEX [IX_ApprovalInstances_RequestedFor] NONCLUSTERED ([RequestedFor]),
        INDEX [IX_ApprovalInstances_DueDate] NONCLUSTERED ([DueDate])
    );

    -- ==================================================================================
    -- 13. WORKFLOW SUSPENSIONS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaWorkflowSuspensions]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [WorkflowInstanceId] NVARCHAR(255) NOT NULL,
        [Reason] NVARCHAR(255),
        [SuspendedBy] NVARCHAR(255),
        [SuspendedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ResumedBy] NVARCHAR(255),
        [ResumedAt] DATETIME2,
        [Notes] NVARCHAR(MAX),
        FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [dbo].[ElsaWorkflowInstances]([Id]) ON DELETE CASCADE,
        INDEX [IX_WorkflowSuspensions_WorkflowInstanceId] NONCLUSTERED ([WorkflowInstanceId]),
        INDEX [IX_WorkflowSuspensions_SuspendedAt] NONCLUSTERED ([SuspendedAt])
    );

    -- ==================================================================================
    -- 14. AUDIT LOGS
    -- ==================================================================================
    CREATE TABLE [dbo].[ElsaAuditLogs]
    (
        [Id] NVARCHAR(255) PRIMARY KEY NOT NULL,
        [EntityType] NVARCHAR(255) NOT NULL,
        [EntityId] NVARCHAR(255) NOT NULL,
        [Action] NVARCHAR(50) NOT NULL,
        [Changes] NVARCHAR(MAX),
        [ChangedBy] NVARCHAR(255),
        [ChangedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IpAddress] NVARCHAR(50),
        [TenantId] NVARCHAR(255),
        INDEX [IX_AuditLogs_EntityType] NONCLUSTERED ([EntityType]),
        INDEX [IX_AuditLogs_EntityId] NONCLUSTERED ([EntityId]),
        INDEX [IX_AuditLogs_ChangedAt] NONCLUSTERED ([ChangedAt])
    );

    -- Unique Constraint
    ALTER TABLE [dbo].[ElsaWorkflowDefinitions]
    ADD CONSTRAINT [UC_WorkflowDefinitions_Name_TenantId] 
    UNIQUE NONCLUSTERED ([Name], [TenantId]);
';
