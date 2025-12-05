SET QUOTED_IDENTIFIER ON;
GO

-- Set user back to Admin (they can access all dashboards now)
UPDATE AspNetUsers 
SET UserType = 'Admin'
WHERE Email = 'superadmin@propertyhubbd.com';
GO

SELECT Email, FullName, UserType, EmailConfirmed 
FROM AspNetUsers 
WHERE Email = 'superadmin@propertyhubbd.com';
GO
