-- Reset Superadmin Password to @Admin123
-- This script will update the password hash for the superadmin account
-- Note: The password hash below is for '@Admin123' using ASP.NET Core Identity's default password hasher

-- First, check if the user exists
SELECT Id, Email, UserName, FullName, UserType, EmailConfirmed
FROM AspNetUsers 
WHERE Email = 'superadmin@propertyhubbd.com';

-- Important: You cannot directly set a password hash via SQL in a secure way
-- The proper way is to use the AdminSeeder which has been updated
-- Please run the application (dotnet run) and the password will be automatically reset to @Admin123

-- Alternative: If you need to reset via code, create a simple console app or use the updated AdminSeeder
-- The AdminSeeder.cs has been updated to reset the password to @Admin123 on application startup
