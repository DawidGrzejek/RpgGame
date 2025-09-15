-- Manual SQL script to check and fix timezone issues
-- This script helps diagnose timezone problems

-- Check current timezone configuration in SQL Server
SELECT CURRENT_TIMEZONE() AS ServerTimezone;

-- Check what time SQL Server thinks it is
SELECT 
    GETDATE() AS ServerLocalTime,
    GETUTCDATE() AS ServerUTCTime,
    SYSDATETIMEOFFSET() AS ServerTimeWithOffset;

-- Check existing user data to see timestamps
SELECT TOP 5 
    Username,
    CreatedAt,
    LastLoginAt,
    Statistics_FirstLoginDate
FROM RpgGame.Users
ORDER BY CreatedAt DESC;

-- Show timezone difference
SELECT 
    Username,
    CreatedAt,
    DATEADD(hour, 2, CreatedAt) AS CreatedAtPlus2Hours,
    Statistics_FirstLoginDate,
    DATEADD(hour, 2, Statistics_FirstLoginDate) AS FirstLoginDatePlus2Hours
FROM RpgGame.Users
WHERE CreatedAt IS NOT NULL;

-- If you need to correct existing data (ONLY RUN IF CONFIRMED ISSUE):
-- UPDATE RpgGame.Users 
-- SET CreatedAt = DATEADD(hour, 2, CreatedAt),
--     Statistics_FirstLoginDate = DATEADD(hour, 2, Statistics_FirstLoginDate)
-- WHERE CreatedAt < GETUTCDATE();