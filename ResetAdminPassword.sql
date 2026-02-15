-- Reset admin user password for AppEnd
-- Run this in SSMS against your AppEnd database
-- Password will be set to: P#ssw0rd (MD5 hash)
-- Supports both BaseUsers and AAA_Users schemas

-- For BaseUsers table (AppEnd default schema):
-- Resets password to P#ssw0rd and unlocks account
UPDATE BaseUsers 
SET Password = 'D7B91B6A9FA705E968B2C859FDDE9457',
    LoginLocked = 0,
    LoginTryFailsCount = 0,
    LoginTryFailLastOn = NULL,
    IsActive = 1
WHERE UserName IN ('admin', 'Admin');

-- If using AAA_Users table (RDBMS-PackageManager schema), uncomment below:
-- UPDATE AAA_Users 
-- SET Password = 'D7B91B6A9FA705E968B2C859FDDE9457',
--     LoginLocked = 0,
--     LoginTryFails = 0,
--     LoginTryFailLastOn = NULL,
--     IsActive = 1
-- WHERE UserName IN ('admin', 'Admin');
-- Note: AAA_Users uses LoginTryFails (not LoginTryFailsCount)

-- Verify user exists:
-- SELECT Id, UserName, IsActive, LoginLocked, LEFT(Password, 8) AS PwdHash FROM BaseUsers WHERE UserName IN ('admin', 'Admin');
