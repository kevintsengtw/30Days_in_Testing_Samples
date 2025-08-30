IF
NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
CREATE TABLE Products
(
    Id            int IDENTITY(1,1) PRIMARY KEY,
    Name          nvarchar(200) NOT NULL,
    Description   nvarchar(1000),
    Price         decimal(18, 2) NOT NULL,
    DiscountPrice decimal(18, 2),
    Stock         int            NOT NULL DEFAULT 0,
    SKU           nvarchar(50),
    IsActive      bit            NOT NULL DEFAULT 1,
    CategoryId    int            NOT NULL,
    CreatedAt     datetime2      NOT NULL DEFAULT GETDATE(),
    UpdatedAt     datetime2,
    FOREIGN KEY (CategoryId) REFERENCES Categories (Id)
)