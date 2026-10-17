-- =========================================================================
-- BÀI TẬP THỰC HÀNH BUỔI 5: STORED PROCEDURES HỆ THỐNG SIÊU THỊ MINI
-- =========================================================================

USE MiniSupermarketDb;
GO

-- 1. STORED PROCEDURE: Báo cáo tổng hợp doanh thu theo nhân viên thu ngân
IF OBJECT_ID('sp_GetRevenueByCashier', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetRevenueByCashier;
GO

CREATE PROCEDURE sp_GetRevenueByCashier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CashierUsername,
        COUNT(OrderId) AS TotalOrders,
        ISNULL(SUM(TotalAmount), 0) AS TotalRevenue,
        ISNULL(SUM(PointsEarned), 0) AS TotalPointsAwarded
    FROM Orders
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC;
END
GO

-- 2. BÀI TẬP MỞ RỘNG 3.1: Báo cáo doanh thu theo khoảng ngày có tham số
IF OBJECT_ID('sp_GetRevenueByDateRange', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetRevenueByDateRange;
GO

CREATE PROCEDURE sp_GetRevenueByDateRange
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CashierUsername,
        COUNT(OrderId) AS TotalOrders,
        ISNULL(SUM(TotalAmount), 0) AS TotalRevenue,
        ISNULL(SUM(PointsEarned), 0) AS TotalPointsAwarded
    FROM Orders
    WHERE OrderDate >= @FromDate AND OrderDate <= @ToDate
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC;
END
GO

-- 3. BÀI TẬP MỞ RỘNG 3.2: Top 5 sản phẩm bán chạy nhất
IF OBJECT_ID('sp_GetTop5SellingProducts', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTop5SellingProducts;
GO

CREATE PROCEDURE sp_GetTop5SellingProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 5
        d.ProductId,
        p.ProductName,
        SUM(d.Quantity) AS TotalQuantitySold,
        SUM(d.Quantity * d.UnitPrice) AS TotalRevenue
    FROM OrderDetails d
    INNER JOIN Products p ON d.ProductId = p.ProductId
    GROUP BY d.ProductId, p.ProductName
    ORDER BY TotalQuantitySold DESC;
END
GO
