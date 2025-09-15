-- Fix GameMaster user roles - ensure GameMaster user has Admin role
-- This script addresses the issue where GameMaster user lacks Admin role access

USE [db24548]; -- Adjust database name if different

-- Check current role assignments for GameMaster user
SELECT 
    u.UserName,
    u.Email,
    r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'GameMaster'
ORDER BY r.Name;

-- Ensure Admin role exists
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
    PRINT 'Created Admin role';
END

-- Ensure GameMaster role exists
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'GameMaster')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'GameMaster', 'GAMEMASTER', NEWID());
    PRINT 'Created GameMaster role';
END

-- Add GameMaster user to Admin role if not already assigned
IF EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = 'GameMaster')
BEGIN
    DECLARE @UserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE UserName = 'GameMaster');
    DECLARE @AdminRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');
    DECLARE @GameMasterRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'GameMaster');
    
    -- Add to Admin role if not already there
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @AdminRoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@UserId, @AdminRoleId);
        PRINT 'Added GameMaster user to Admin role';
    END
    ELSE
    BEGIN
        PRINT 'GameMaster user already has Admin role';
    END
    
    -- Add to GameMaster role if not already there
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @GameMasterRoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@UserId, @GameMasterRoleId);
        PRINT 'Added GameMaster user to GameMaster role';
    END
    ELSE
    BEGIN
        PRINT 'GameMaster user already has GameMaster role';
    END
END
ELSE
BEGIN
    PRINT 'GameMaster user not found - please run application seeding first';
END

-- Verify final role assignments
SELECT 
    u.UserName,
    u.Email,
    r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'GameMaster'
ORDER BY r.Name;