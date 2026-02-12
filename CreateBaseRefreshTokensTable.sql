-- Create BaseRefreshTokens Table
-- Stores refresh tokens for Refresh Token Rotation (httpOnly cookie auth)

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BaseRefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BaseRefreshTokens] (
        [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [TokenHash] NVARCHAR(256) NOT NULL,
        [ExpiryDate] DATETIME NOT NULL,
        [IsRevoked] BIT NOT NULL DEFAULT 0,
        [CreatedOn] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_BaseRefreshTokens_BaseUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[BaseUsers]([Id])
    );
    
    CREATE INDEX IX_BaseRefreshTokens_TokenHash ON [dbo].[BaseRefreshTokens]([TokenHash]);
    CREATE INDEX IX_BaseRefreshTokens_UserId ON [dbo].[BaseRefreshTokens]([UserId]);
    CREATE INDEX IX_BaseRefreshTokens_ExpiryDate ON [dbo].[BaseRefreshTokens]([ExpiryDate]);
END
GO
