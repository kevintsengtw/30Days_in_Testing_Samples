IF
NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
CREATE TABLE Categories
(
    Id          int IDENTITY(1,1) PRIMARY KEY,
    Name        nvarchar(100) NOT NULL,
    Description nvarchar(500),
    IsActive    bit       NOT NULL DEFAULT 1,
    CreatedAt   datetime2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt   datetime2
)