-- =============================================
-- Seed Admin User for AppEnd
-- Run in SSMS against your AppEnd database
-- Creates: admin role, admin user (Password: P#ssw0rd), and links them
--
-- Prerequisite: BaseUsers, BaseRoles, BaseUsersRoles tables must exist.
-- If you get "Invalid object name" error, run Zzz_Deploy first to create schema.
-- =============================================

SET NOCOUNT ON;

-- 1) Insert admin role if not exists
IF NOT EXISTS (SELECT 1 FROM BaseRoles WHERE RoleName = 'admin')
BEGIN
    SET IDENTITY_INSERT BaseRoles ON;
    INSERT INTO BaseRoles (Id, CreatedBy, CreatedOn, IsBuiltIn, RoleName, Note, IsActive)
    VALUES (100, 100000, GETDATE(), 1, 'admin', N'Administrator role', 1);
    SET IDENTITY_INSERT BaseRoles OFF;
    PRINT 'BaseRoles: admin role created.';
END
ELSE
    PRINT 'BaseRoles: admin role already exists.';

-- 2) Insert admin user if not exists
IF NOT EXISTS (SELECT 1 FROM BaseUsers WHERE UserName IN ('admin', 'Admin'))
BEGIN
    SET IDENTITY_INSERT BaseUsers ON;
    INSERT INTO BaseUsers (
        Id, CreatedBy, CreatedOn, IsBuiltIn, UserName, Email, Mobile,
        Password, IsActive, LoginLocked, LoginTryFailsCount, Settings
    )
    VALUES (
        100000, 100000, GETDATE(), 1, 'admin', 'admin@localhost', NULL,
        'D7B91B6A9FA705E968B2C859FDDE9457',  -- MD5 of P#ssw0rd
        1, 0, 0, '{}'
    );
    SET IDENTITY_INSERT BaseUsers OFF;
    PRINT 'BaseUsers: admin user created.';
END
ELSE
    PRINT 'BaseUsers: admin user already exists.';

-- 3) Link admin user to admin role if not already linked
IF NOT EXISTS (
    SELECT 1 FROM BaseUsersRoles ur
    INNER JOIN BaseUsers u ON ur.UserId = u.Id
    INNER JOIN BaseRoles r ON ur.RoleId = r.Id
    WHERE u.UserName IN ('admin', 'Admin') AND r.RoleName = 'admin'
)
BEGIN
    INSERT INTO BaseUsersRoles (CreatedBy, CreatedOn, UserId, RoleId)
    SELECT 100000, GETDATE(), u.Id, r.Id
    FROM BaseUsers u
    CROSS JOIN BaseRoles r
    WHERE u.UserName IN ('admin', 'Admin') AND r.RoleName = 'admin';
    PRINT 'BaseUsersRoles: admin user linked to admin role.';
END
ELSE
    PRINT 'BaseUsersRoles: link already exists.';

-- Verify
SELECT u.Id, u.UserName, u.IsActive, u.LoginLocked, r.RoleName
FROM BaseUsers u
LEFT JOIN BaseUsersRoles ur ON ur.UserId = u.Id
LEFT JOIN BaseRoles r ON ur.RoleId = r.Id
WHERE u.UserName IN ('admin', 'Admin');

PRINT '';
PRINT 'Done. Login with: admin / P#ssw0rd';
