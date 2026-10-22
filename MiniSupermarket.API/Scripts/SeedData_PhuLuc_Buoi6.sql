-- =========================================================================
-- PHỤ LỤC & BUỔI 6: DATA SEEDING TOÀN DIỆN CHO CSDL MINISUPERMARKETDB
-- =========================================================================

USE MiniSupermarketDb;
GO

-- 1. Bổ sung Category 2 nếu chưa có
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = 2)
BEGIN
    SET IDENTITY_INSERT Categories ON;
    INSERT INTO Categories (CategoryId, CategoryName, Description)
    VALUES (2, N'Nước giải khát & Trà', N'Nước ngọt lon, nước khoáng thiên nhiên, trà xanh đóng chai');
    SET IDENTITY_INSERT Categories OFF;
END
GO

-- 2. Bổ sung 20 sản phẩm mẫu chuẩn mực của Phụ lục và Buổi 6
DECLARE @SampleProducts TABLE (
    Barcode NVARCHAR(50),
    ProductName NVARCHAR(150),
    Price DECIMAL(18,2),
    StockQuantity INT,
    CategoryId INT
);

INSERT INTO @SampleProducts (Barcode, ProductName, Price, StockQuantity, CategoryId) VALUES
('893456789001', N'Snack Khoai Tây O''Star Gói 60g', 12500, 150, 1),
('893456789002', N'Bánh Quy Bơ Danisa Hộp Thiếc 454g', 135000, 40, 1),
('893456789003', N'Kẹo Dẻo Chupa Chups Hương Trái Cây 90g', 15000, 120, 1),
('893456789004', N'Bánh Chocopie Orion Hộp 12 Cái', 54000, 60, 1),
('893456789005', N'Nước Ngọt Coca Cola Lon 320ml', 10000, 320, 2),
('893456789006', N'Nước Khoáng Thiên Nhiên Lavie Chai 500ml', 6000, 200, 2),
('893456789007', N'Trà Xanh Không Độ Chai 455ml', 11000, 180, 2),
('893456789008', N'Nước Tăng Lực Red Bull Lon 250ml', 14500, 150, 2),
('893456789009', N'Sữa Tươi Tiệt Trùng Vinamilk Có Đường 1L', 34500, 85, 3),
('893456789010', N'Phô Mai Con Bò Cười Hộp 8 Miếng', 39000, 60, 3),
('893456789011', N'Sữa Chua Ăn Vinamilk Có Đường Lốc 4 Hộp', 28000, 95, 3),
('893456789012', N'Sữa Đặc Có Đường Ông Thọ Lon 380g', 26000, 110, 3),
('893456789013', N'Mì Tôm Hảo Hảo Chua Cay Gói 75g', 4500, 500, 4),
('893456789014', N'Phở Bò Đệ Nhất Gói 68g', 8500, 140, 4),
('893456789015', N'Mì Trộn Tương Đen Koreno Gói 115g', 14000, 80, 4),
('893456789016', N'Dầu Ăn Simply Nguyên Chất Can 2L', 125000, 45, 5),
('893456789017', N'Nước Mắm Nam Ngư Đệ Nhị Chai 900ml', 30000, 90, 5),
('893456789018', N'Hạt Nêm Knorr Từ Thịt Thăn Gói 400g', 36000, 75, 5),
('893456789019', N'Cá Nục Sốt Cà Ba Cô Gái Hộp 155g', 19000, 130, 6),
('893456789020', N'Nước Rửa Chén Sunlight Chanh Chai 750g', 29000, 80, 7);

INSERT INTO Products (Barcode, ProductName, Price, StockQuantity, CategoryId)
SELECT s.Barcode, s.ProductName, s.Price, s.StockQuantity, s.CategoryId
FROM @SampleProducts s
WHERE NOT EXISTS (SELECT 1 FROM Products p WHERE p.Barcode = s.Barcode);
GO

-- 3. Đảm bảo Orders có đủ 15 dòng mẫu chuẩn
WHILE (SELECT COUNT(*) FROM Orders) < 15
BEGIN
    INSERT INTO Orders (OrderDate, CashierUsername, CustomerId, TotalAmount, PointsEarned)
    VALUES (
        DATEADD(day, -ROUND(RAND() * 10, 0), GETDATE()),
        'cashier0' + CAST(CAST(ROUND(RAND() * 4 + 1, 0) AS INT) AS NVARCHAR),
        CAST(ROUND(RAND() * 14 + 1, 0) AS INT),
        ROUND(RAND() * 200000 + 50000, -3),
        CAST(ROUND(RAND() * 20 + 5, 0) AS INT)
    );
END
GO
