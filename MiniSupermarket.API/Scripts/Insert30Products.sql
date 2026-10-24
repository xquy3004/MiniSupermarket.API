USE MiniSupermarketDb;
GO

SET IDENTITY_INSERT Products ON;

INSERT INTO Products (ProductId, Barcode, ProductName, Price, StockQuantity, CategoryId)
VALUES
(21, '893456789021', N'Sữa bột Meiji số 0 (0-1 tuổi) hộp 800g nhập khẩu Nhật', 530000.0, 35, 1),
(22, '893456789022', N'Sữa bột Frisolac Gold số 2 (6-12 tháng) hộp 850g', 465000.0, 15, 1),
(23, '893456789023', N'Sữa tươi TH True Milk TOP KID hương dâu 180ml lốc 4', 36000.0, 120, 2),
(24, '893456789024', N'Sữa chua uống tiệt trùng Yomost vị cam lốc 4 hộp 170ml', 28000.0, 80, 2),
(25, '893456789025', N'Sữa hạt ngũ cốc dinh dưỡng Pororo Hàn Quốc lốc 3 hộp 190ml', 55000.0, 40, 2),
(26, '893456789026', N'Tã dán Merries nội địa Nhật size M (6-11kg) gói 64 miếng', 385000.0, 18, 3),
(27, '893456789027', N'Tã quần Moony Natural hữu cơ size L (9-14kg) gói 36 miếng', 410000.0, 25, 3),
(28, '893456789028', N'Khăn giấy khô đa năng siêu mềm Mama hộp 200 tờ', 35000.0, 150, 3),
(29, '893456789029', N'Bánh ăn dặm Gerber Graduate vị chuối dâu hộp 42g', 68000.0, 70, 4),
(30, '893456789030', N'Cháo tươi Cây Thị lươn đậu xanh dinh dưỡng gói 260g', 25000.0, 90, 4),
(31, '893456789031', N'Váng sữa Monte vị vani nhập khẩu Đức lốc 4 hộp 55g', 62000.0, 110, 4),
(32, '893456789032', N'Núm ty thay thế Pigeon siêu mềm cổ rộng size M vỉ 2 cái', 120000.0, 50, 5),
(33, '893456789033', N'Ti giả chỉnh nha Philips Avent Ultra Air ban đêm vỉ 2 cái', 175000.0, 10, 5),
(34, '893456789034', N'Nước rửa bình sữa và rau củ D-nee chai có vòi 620ml', 85000.0, 65, 5),
(35, '893456789035', N'Bộ chén muỗng tập ăn dặm báo nóng Richell Nhật Bản', 145000.0, 40, 6),
(36, '893456789036', N'Ghế ăn dặm gấp gọn nâng hạ độ cao Mastela cho bé', 490000.0, 8, 6),
(37, '893456789037', N'Yếm ăn dặm silicon cao cấp chống thấm có máng hứng', 55000.0, 85, 6),
(38, '893456789038', N'Phấn rôm tinh chất hoa cúc Chicco 150g chống rôm sảy', 110000.0, 45, 7),
(39, '893456789039', N'Sữa dưỡng ẩm toàn thân Cetaphil Baby Daily Lotion 400ml', 235000.0, 12, 7),
(40, '893456789040', N'Bàn chải đánh răng xỏ ngón silicon siêu mềm Richell', 45000.0, 90, 8),
(41, '893456789041', N'Kem đánh răng hữu cơ nuốt được Azetabio hương táo 50ml', 165000.0, 35, 8),
(42, '893456789042', N'Gạc rơ lưỡi tưa lưỡi thảo dược Dr.Papie hộp 30 gói', 95000.0, 130, 8),
(43, '893456789043', N'Bộ bao tay bao chân sơ sinh sợi tre Miomio kháng khuẩn', 35000.0, 100, 9),
(44, '893456789044', N'Mũ chụp sơ sinh bo viền Nous chất liệu Nu Petit co giãn', 45000.0, 60, 9),
(45, '893456789045', N'Gối chặn bông hạt xốp chống giật mình Uala Rogo cho bé', 185000.0, 22, 10),
(46, '893456789046', N'Khăn quấn ủ kén sơ sinh SwaddleMe cotton co giãn 4 chiều', 135000.0, 14, 10),
(47, '893456789047', N'Xe đẩy du lịch gấp gọn siêu nhẹ Seebaby QQ3 thoáng khí', 890000.0, 6, 12),
(48, '893456789048', N'Địu em bé trợ lực 4 tư thế Ergobaby Omni 360 Cool Air', 1250000.0, 5, 12),
(49, '893456789049', N'Đai nịt bụng định hình thoáng khí sau sinh Farlin', 290000.0, 18, 14),
(50, '893456789050', N'Máy hút sữa điện đôi Spectra 9 Plus có pin tích điện', 235000.0, 9, 15);

SET IDENTITY_INSERT Products OFF;
GO
