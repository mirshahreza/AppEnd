-- Create BaseDbConnections Table
-- This table stores database connection strings and enrichment status

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BaseDbConnections]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BaseDbConnections] (
        [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [CreatedBy] INT NOT NULL,
        [CreatedOn] DATETIME NOT NULL,
        [UpdatedBy] INT NULL,
        [UpdatedOn] DATETIME NULL,
        [IsActive] BIT NULL,
        
        -- Connection specific fields
        [Name] NVARCHAR(200) NOT NULL,
        [ServerType] VARCHAR(50) NOT NULL, -- MsSql, PostgreSql, MySql, Oracle
        [ConnectionString] NVARCHAR(2000) NOT NULL,
        
        -- Enrichment specific fields
        [Status] VARCHAR(50) NULL, -- not_enriched, enriching, enriched
        [EnrichmentProgress] INT NULL DEFAULT 0, -- 0-100
        [LastUpdated] DATETIME NULL
    );
    
    -- Create index on Name for faster lookups
    CREATE INDEX IX_BaseDbConnections_Name ON [dbo].[BaseDbConnections]([Name]);
    
    -- Create index on Status for filtering
    CREATE INDEX IX_BaseDbConnections_Status ON [dbo].[BaseDbConnections]([Status]);
END
GO

