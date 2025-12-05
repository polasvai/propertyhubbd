-- First, check if the user exists
SELECT Id, Email, UserName, FullName, UserType, EmailConfirmed
FROM AspNetUsers 
WHERE Email = 'superadmin@propertyhubbd.com';

-- If the user exists, update to Admin
UPDATE AspNetUsers 
SET UserType = 'Admin', 
    FullName = 'Super Admin',
    EmailConfirmed = 1
WHERE Email = 'superadmin@propertyhubbd.com';

-- Verify the update
SELECT Id, Email, UserName, FullName, UserType, EmailConfirmed
FROM AspNetUsers 
WHERE Email = 'superadmin@propertyhubbd.com';
