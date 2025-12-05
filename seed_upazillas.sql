SET QUOTED_IDENTIFIER ON;
GO

-- Get District IDs
DECLARE @DhakaDistrictId INT;
SELECT @DhakaDistrictId = Id FROM Districts WHERE Name = 'Dhaka' AND DivisionId = (SELECT Id FROM Divisions WHERE Name = 'Dhaka');

DECLARE @GazipurDistrictId INT;
SELECT @GazipurDistrictId = Id FROM Districts WHERE Name = 'Gazipur';

DECLARE @NarayanganjDistrictId INT;
SELECT @NarayanganjDistrictId = Id FROM Districts WHERE Name = 'Narayanganj';

-- Insert Upazillas for Dhaka District
INSERT INTO Upazillas (Name, DistrictId) VALUES
('Dhanmondi', @DhakaDistrictId),
('Gulshan', @DhakaDistrictId),
('Mirpur', @DhakaDistrictId),
('Uttara', @DhakaDistrictId),
('Mohammadpur', @DhakaDistrictId),
('Banani', @DhakaDistrictId);

-- Insert Upazillas for Gazipur
INSERT INTO Upazillas (Name, DistrictId) VALUES
('Gazipur Sadar', @GazipurDistrictId),
('Kaliakair', @GazipurDistrictId),
('Kapasia', @GazipurDistrictId);

-- Insert Upazillas for Narayanganj
INSERT INTO Upazillas (Name, DistrictId) VALUES
('Narayanganj Sadar', @NarayanganjDistrictId),
('Rupganj', @NarayanganjDistrictId),
('Sonargaon', @NarayanganjDistrictId);

GO

-- Verify
SELECT 'Upazillas:' as TableName, COUNT(*) as RecordCount FROM Upazillas;
GO

SELECT u.Name as Upazilla, d.Name as District, dv.Name as Division
FROM Upazillas u
JOIN Districts d ON u.DistrictId = d.Id
JOIN Divisions dv ON d.DivisionId = dv.Id
ORDER BY dv.Name, d.Name, u.Name;
GO
