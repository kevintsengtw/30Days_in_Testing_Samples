IF
NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
CREATE TABLE Orders
(
    Id            int IDENTITY(1,1) PRIMARY KEY,
    OrderNumber   nvarchar(50) NOT NULL,
    CustomerId    int            NOT NULL,
    CustomerName  nvarchar(100) NOT NULL,
    CustomerEmail nvarchar(255) NOT NULL,
    OrderDate     datetime2      NOT NULL DEFAULT GETUTCDATE(),
    Status        nvarchar(20) NOT NULL DEFAULT 'Pending',
    TotalAmount   decimal(18, 2) NOT NULL,
    IsActive      bit            NOT NULL DEFAULT 1,
    CreatedAt     datetime2      NOT NULL DEFAULT GETDATE(),
    UpdatedAt     datetime2,
    FOREIGN KEY (CustomerId) REFERENCES Customers (Id)
)