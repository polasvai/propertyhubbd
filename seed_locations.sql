SET QUOTED_IDENTIFIER ON;
GO

-- Insert Divisions
INSERT INTO Divisions (Name, Color, SvgPath) VALUES
('Dhaka', '#FF5733', ''),
('Chittagong', '#33FF57', ''),
('Rajshahi', '#3357FF', ''),
('Khulna', '#F333FF', ''),
('Barisal', '#FF33A1', ''),
('Sylhet', '#33FFF3', ''),
('Rangpur', '#F3FF33', ''),
('Mymensingh', '#FF8C33', '');
GO

-- Get Dhaka Division ID
DECLARE @DhakaId INT;
SELECT @DhakaId = Id FROM Divisions WHERE Name = 'Dhaka';

-- Insert Districts for Dhaka
INSERT INTO Districts (Name, DivisionId) VALUES
('Dhaka', @DhakaId),
('Gazipur', @DhakaId),
('Narayanganj', @DhakaId),
('Tangail', @DhakaId),
('Manikganj', @DhakaId);
GO

-- Verify
SELECT 'Divisions:' as TableName, COUNT(*) as RecordCount FROM Divisions
UNION ALL
SELECT 'Districts:', COUNT(*) FROM Districts;
GO

SELECT * FROM Divisions;
GO
