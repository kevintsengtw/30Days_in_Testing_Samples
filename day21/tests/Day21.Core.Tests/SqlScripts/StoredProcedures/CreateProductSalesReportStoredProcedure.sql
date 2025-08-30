-- 建立產品銷售報表預存程序
IF
OBJECT_ID('sp_GetProductSalesReport', 'P') IS NOT NULL
DROP PROCEDURE sp_GetProductSalesReport;

IF
OBJECT_ID('sp_GetProductSalesReport', 'P') IS NULL
BEGIN
EXEC('CREATE PROCEDURE sp_GetProductSalesReport
        @MinPrice decimal(18,2)
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT 
            p.Name,
            SUM(oi.Quantity) as TotalQuantity,
            SUM(oi.Quantity * oi.UnitPrice) as TotalRevenue
        FROM Products p
        INNER JOIN OrderItems oi ON p.Id = oi.ProductId
        WHERE p.Price >= @MinPrice
        GROUP BY p.Name
        ORDER BY TotalRevenue DESC;
    END');
END
