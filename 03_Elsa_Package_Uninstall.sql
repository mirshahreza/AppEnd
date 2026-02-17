-- ==================================================================================
-- Elsa 3.0 Package - Uninstallation Script
-- ==================================================================================
-- این اسکریپت برای حذف Elsa از دیتابیس استفاده می‌شود
-- ==================================================================================

-- حذف جداول (ترتیب مهم است - Foreign Key‌ها را در نظر بگیر)

IF OBJECT_ID('dbo.ElsaAuditLogs', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaAuditLogs];

IF OBJECT_ID('dbo.ElsaApprovalInstances', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaApprovalInstances];

IF OBJECT_ID('dbo.ElsaWorkflowSuspensions', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaWorkflowSuspensions];

IF OBJECT_ID('dbo.ElsaWorkflowEvents', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaWorkflowEvents];

IF OBJECT_ID('dbo.ElsaExecutionContexts', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaExecutionContexts];

IF OBJECT_ID('dbo.ElsaWorkflowTriggers', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaWorkflowTriggers];

IF OBJECT_ID('dbo.ElsaTriggeredWorkflows', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaTriggeredWorkflows];

IF OBJECT_ID('dbo.ElsaVariableInstances', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaVariableInstances];

IF OBJECT_ID('dbo.ElsaWorkflowExecutionLogs', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaWorkflowExecutionLogs];

IF OBJECT_ID('dbo.ElsaBookmarks', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaBookmarks];

IF OBJECT_ID('dbo.ElsaActivityExecutions', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaActivityExecutions];

IF OBJECT_ID('dbo.ElsaWorkflowInstances', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaWorkflowInstances];

IF OBJECT_ID('dbo.ElsaWorkflowDefinitionVersions', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaWorkflowDefinitionVersions];

IF OBJECT_ID('dbo.ElsaWorkflowDefinitions', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElsaWorkflowDefinitions];

PRINT 'Elsa 3.0 جداول با موفقیت حذف شدند.';
PRINT 'Elsa 3.0 tables successfully removed.';
