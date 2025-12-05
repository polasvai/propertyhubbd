SET QUOTED_IDENTIFIER ON;
GO

-- Update admin to also be a Seller so they can add properties
UPDATE AspNetUsers 
SET UserType = 'Seller'
WHERE Email = 'superadmin@propertyhubbd.com';
GO

SELECT Email, FullName, UserType, EmailConfirmed 
FROM AspNetUsers 
WHERE Email = 'superadmin@propertyhubbd.com';
GO
