SET QUOTED_IDENTIFIER ON;
GO

UPDATE AspNetUsers 
SET UserType = 'Admin', 
    FullName = 'Super Admin',
    EmailConfirmed = 1
WHERE Email = 'superadmin@propertyhubbd.com';
GO

SELECT Email, FullName, UserType, EmailConfirmed 
FROM AspNetUsers 
WHERE Email = 'superadmin@propertyhubbd.com';
GO
