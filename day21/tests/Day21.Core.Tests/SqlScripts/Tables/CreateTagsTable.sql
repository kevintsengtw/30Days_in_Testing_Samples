-- 建立 Tags 表格
IF
NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tags' AND xtype='U')
BEGIN
CREATE TABLE Tags
(
    Id          int IDENTITY(1,1) PRIMARY KEY,
    Name        nvarchar(100) NOT NULL,
    Description nvarchar(500) NULL,
    IsActive    bit       NOT NULL DEFAULT 1,
    CreatedAt   datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   datetime2 NULL
);
END
