IF
NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderItems' AND xtype='U')
CREATE TABLE OrderItems
(
    Id        int IDENTITY(1,1) PRIMARY KEY,
    OrderId   int            NOT NULL,
    ProductId int            NOT NULL,
    Quantity  int            NOT NULL,
    UnitPrice decimal(18, 2) NOT NULL,
    Subtotal  decimal(18, 2) NOT NULL,
    CreatedAt datetime2      NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (OrderId) REFERENCES Orders (Id),
    FOREIGN KEY (ProductId) REFERENCES Products (Id)
)
