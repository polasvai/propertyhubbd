-- Update a user to be an Admin
-- Replace 'your-email@example.com' with the actual email address

UPDATE AspNetUsers 
SET UserType = 'Admin' 
WHERE Email = 'your-email@example.com';

-- Verify the change
SELECT Id, Email, UserName, FullName, UserType 
FROM AspNetUsers 
WHERE Email = 'your-email@example.com';
