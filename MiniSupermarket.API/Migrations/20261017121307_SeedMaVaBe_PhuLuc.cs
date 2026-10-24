using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedMaVaBe_PhuLuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                column: "CategoryName",
                value: "Sữa tươi & Sữa dinh dưỡng bé");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8,
                column: "CategoryName",
                value: "Vệ sinh & Chăm sóc răng miệng");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 13,
                column: "CategoryName",
                value: "Dinh dưỡng & Vitamin mẹ bầu");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 14,
                column: "Description",
                value: "Đai bụng, kem rạn da, miếng lót sau sinh, đồ dùng mẹ");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Nguyễn Thị Lan Anh", 350 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Trần Thị Thu Hương", 120 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Lê Thị Bích Ngọc", 25 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4,
                columns: new[] { "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Phạm Thị Thanh Tâm", "Bạch Kim", "0903456789", 520 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Hoàng Thị Minh Châu", 210 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6,
                column: "CustomerName",
                value: "Đỗ Thị Kim Liên");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 7,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Vũ Thị Hồng Nhung", 15 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 8,
                columns: new[] { "CustomerName", "MembershipRank", "RewardPoints" },
                values: new object[] { "Ngô Thị Mỹ Linh", "Bạch Kim", 480 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 9,
                column: "CustomerName",
                value: "Bùi Thị Phương Anh");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 10,
                column: "CustomerName",
                value: "Đặng Thị Thanh Mai");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 11,
                column: "CustomerName",
                value: "Dương Thị Quỳnh Anh");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 12,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Lý Thị Ngọc Hà", 265 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 13,
                column: "CustomerName",
                value: "Trương Thị Cẩm Vân");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 14,
                column: "MembershipRank",
                value: "Bạch Kim");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 15,
                column: "CustomerName",
                value: "Huỳnh Thị Gia Linh");

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "CashierUsername", "CustomerId", "OrderDate", "PointsEarned", "TotalAmount" },
                values: new object[,]
                {
                    { 1, "cashier01", 1, new DateTime(2026, 10, 1, 8, 30, 0, 0, DateTimeKind.Unspecified), 38, 380000m },
                    { 2, "cashier01", 2, new DateTime(2026, 10, 1, 9, 15, 0, 0, DateTimeKind.Unspecified), 52, 520000m },
                    { 3, "cashier02", 3, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 27, 270000m },
                    { 4, "cashier02", 4, new DateTime(2026, 10, 2, 8, 45, 0, 0, DateTimeKind.Unspecified), 68, 685000m },
                    { 5, "cashier03", 5, new DateTime(2026, 10, 2, 11, 20, 0, 0, DateTimeKind.Unspecified), 18, 185000m },
                    { 6, "cashier03", 6, new DateTime(2026, 10, 3, 14, 10, 0, 0, DateTimeKind.Unspecified), 45, 450000m },
                    { 7, "cashier04", 7, new DateTime(2026, 10, 3, 16, 40, 0, 0, DateTimeKind.Unspecified), 12, 125000m },
                    { 8, "cashier04", 8, new DateTime(2026, 10, 4, 9, 5, 0, 0, DateTimeKind.Unspecified), 97, 970000m },
                    { 9, "cashier05", 9, new DateTime(2026, 10, 4, 13, 30, 0, 0, DateTimeKind.Unspecified), 33, 330000m },
                    { 10, "cashier05", 10, new DateTime(2026, 10, 5, 10, 50, 0, 0, DateTimeKind.Unspecified), 24, 245000m },
                    { 11, "cashier06", 11, new DateTime(2026, 10, 5, 17, 15, 0, 0, DateTimeKind.Unspecified), 19, 195000m },
                    { 12, "cashier06", 12, new DateTime(2026, 10, 6, 8, 20, 0, 0, DateTimeKind.Unspecified), 101, 1010000m },
                    { 13, "cashier07", 13, new DateTime(2026, 10, 6, 15, 0, 0, 0, DateTimeKind.Unspecified), 28, 280000m },
                    { 14, "cashier07", 14, new DateTime(2026, 10, 7, 12, 10, 0, 0, DateTimeKind.Unspecified), 76, 760000m },
                    { 15, "cashier01", 15, new DateTime(2026, 10, 7, 18, 45, 0, 0, DateTimeKind.Unspecified), 16, 165000m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "893456789001", 3, 195000m, "Tã dán Pampers Newborn size NB (dưới 5kg) – 84 miếng", 85 },
                    { 2, "893456789002", 3, 185000m, "Tã quần Huggies Dry size M (6-11kg) – 48 miếng", 70 },
                    { 3, "893456789003", 3, 135000m, "Tã dán Bobby Newborn size S (3-6kg) – 56 miếng", 120 },
                    { 4, "893456789004", 3, 42000m, "Khăn ướt Bobby không mùi hộp 100 tờ", 200 },
                    { 5, "893456789005", 1, 520000m, "Sữa bột NAN Optipro 1 (0-6 tháng) hộp 800g", 45 },
                    { 6, "893456789006", 1, 490000m, "Sữa bột Enfamil A+ số 2 (6-12 tháng) hộp 900g", 50 },
                    { 7, "893456789007", 1, 480000m, "Sữa bột Aptamil số 3 (1-3 tuổi) hộp 800g", 40 },
                    { 8, "893456789008", 1, 320000m, "Sữa bột Dielac Alpha Step 1 (0-6 tháng) hộp 900g", 60 },
                    { 9, "893456789009", 4, 85000m, "Bột ăn dặm Heinz vị gạo sữa loại 200g", 90 },
                    { 10, "893456789010", 4, 38000m, "Cháo tươi ăn dặm Nutricare gà rau củ hộp 200g", 150 },
                    { 11, "893456789011", 4, 65000m, "Váng sữa Petit Suisse Yoplait vị dâu lốc 6 hộp", 100 },
                    { 12, "893456789012", 5, 185000m, "Bình sữa Pigeon PPSU cổ rộng 160ml", 55 },
                    { 13, "893456789013", 5, 245000m, "Bình sữa Dr. Brown's chống sặc 250ml", 35 },
                    { 14, "893456789014", 7, 95000m, "Sữa tắm gội Pigeon 2 trong 1 hương hoa quả 200ml", 110 },
                    { 15, "893456789015", 7, 125000m, "Kem chống hăm Bepanthen Baby 30g", 80 },
                    { 16, "893456789016", 7, 78000m, "Dầu massage Johnson's Baby 200ml", 95 },
                    { 17, "893456789017", 9, 280000m, "Bộ bodysuit cotton sơ sinh 5 bộ size 0-3 tháng", 60 },
                    { 18, "893456789018", 13, 450000m, "Sữa bầu Enfamama A+ hộp 900g", 40 },
                    { 19, "893456789019", 13, 380000m, "Vitamin tổng hợp Blackmores Pregnancy & Breast-feeding 60 viên", 30 },
                    { 20, "893456789020", 11, 165000m, "Bộ đồ chơi xúc xắc gặm nướu an toàn cho bé 8 món", 75 },
                    { 21, "893456789021", 1, 530000m, "Sữa bột Meiji số 0 (0-1 tuổi) hộp 800g nhập khẩu Nhật", 35 },
                    { 22, "893456789022", 1, 465000m, "Sữa bột Frisolac Gold số 2 (6-12 tháng) hộp 850g", 15 },
                    { 23, "893456789023", 2, 36000m, "Sữa tươi TH True Milk TOP KID hương dâu 180ml lốc 4", 120 },
                    { 24, "893456789024", 2, 28000m, "Sữa chua uống tiệt trùng Yomost vị cam lốc 4 hộp 170ml", 80 },
                    { 25, "893456789025", 2, 55000m, "Sữa hạt ngũ cốc dinh dưỡng Pororo Hàn Quốc lốc 3 hộp 190ml", 40 },
                    { 26, "893456789026", 3, 385000m, "Tã dán Merries nội địa Nhật size M (6-11kg) gói 64 miếng", 18 },
                    { 27, "893456789027", 3, 410000m, "Tã quần Moony Natural hữu cơ size L (9-14kg) gói 36 miếng", 25 },
                    { 28, "893456789028", 3, 35000m, "Khăn giấy khô đa năng siêu mềm Mama hộp 200 tờ", 150 },
                    { 29, "893456789029", 4, 68000m, "Bánh ăn dặm Gerber Graduate vị chuối dâu hộp 42g", 70 },
                    { 30, "893456789030", 4, 25000m, "Cháo tươi Cây Thị lươn đậu xanh dinh dưỡng gói 260g", 90 },
                    { 31, "893456789031", 4, 62000m, "Váng sữa Monte vị vani nhập khẩu Đức lốc 4 hộp 55g", 110 },
                    { 32, "893456789032", 5, 120000m, "Núm ty thay thế Pigeon siêu mềm cổ rộng size M vỉ 2 cái", 50 },
                    { 33, "893456789033", 5, 175000m, "Ti giả chỉnh nha Philips Avent Ultra Air ban đêm vỉ 2 cái", 10 },
                    { 34, "893456789034", 5, 85000m, "Nước rửa bình sữa và rau củ D-nee chai có vòi 620ml", 65 },
                    { 35, "893456789035", 6, 145000m, "Bộ chén muỗng tập ăn dặm báo nóng Richell Nhật Bản", 40 },
                    { 36, "893456789036", 6, 490000m, "Ghế ăn dặm gấp gọn nâng hạ độ cao Mastela cho bé", 8 },
                    { 37, "893456789037", 6, 55000m, "Yếm ăn dặm silicon cao cấp chống thấm có máng hứng", 85 },
                    { 38, "893456789038", 7, 110000m, "Phấn rôm tinh chất hoa cúc Chicco 150g chống rôm sảy", 45 },
                    { 39, "893456789039", 7, 235000m, "Sữa dưỡng ẩm toàn thân Cetaphil Baby Daily Lotion 400ml", 12 },
                    { 40, "893456789040", 8, 45000m, "Bàn chải đánh răng xỏ ngón silicon siêu mềm Richell", 90 },
                    { 41, "893456789041", 8, 165000m, "Kem đánh răng hữu cơ nuốt được Azetabio hương táo 50ml", 35 },
                    { 42, "893456789042", 8, 95000m, "Gạc rơ lưỡi tưa lưỡi thảo dược Dr.Papie hộp 30 gói", 130 },
                    { 43, "893456789043", 9, 35000m, "Bộ bao tay bao chân sơ sinh sợi tre Miomio kháng khuẩn", 100 },
                    { 44, "893456789044", 9, 45000m, "Mũ chụp sơ sinh bo viền Nous chất liệu Nu Petit co giãn", 60 },
                    { 45, "893456789045", 10, 185000m, "Gối chặn bông hạt xốp chống giật mình Uala Rogo cho bé", 22 },
                    { 46, "893456789046", 10, 135000m, "Khăn quấn ủ kén sơ sinh SwaddleMe cotton co giãn 4 chiều", 14 },
                    { 47, "893456789047", 12, 890000m, "Xe đẩy du lịch gấp gọn siêu nhẹ Seebaby QQ3 thoáng khí", 6 },
                    { 48, "893456789048", 12, 1250000m, "Địu em bé trợ lực 4 tư thế Ergobaby Omni 360 Cool Air", 5 },
                    { 49, "893456789049", 14, 290000m, "Đai nịt bụng định hình thoáng khí sau sinh Farlin", 18 },
                    { 50, "893456789050", 15, 2350000m, "Máy hút sữa điện đôi Spectra 9 Plus có pin tích điện", 9 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Nguyễn Thị Quản Trị", true, "admin@123", "Admin", "admin" },
                    { 2, "Trần Cửa Hàng Trưởng", true, "manager@123", "Admin", "manager01" },
                    { 3, "Lê Thị Thu Ngân 1", true, "cashier@123", "Cashier", "cashier01" },
                    { 4, "Phạm Thị Thu Ngân 2", true, "cashier@123", "Cashier", "cashier02" },
                    { 5, "Hoàng Thị Thu Ngân 3", true, "cashier@123", "Cashier", "cashier03" },
                    { 6, "Vũ Thị Thu Ngân 4", true, "cashier@123", "Cashier", "cashier04" },
                    { 7, "Đỗ Thị Thu Ngân 5", true, "cashier@123", "Cashier", "cashier05" },
                    { 8, "Ngô Thị Thu Ngân 6", true, "cashier@123", "Cashier", "cashier06" },
                    { 9, "Bùi Thị Thu Ngân 7", true, "cashier@123", "Cashier", "cashier07" },
                    { 10, "Đặng Văn Thủ Kho 1", true, "wh@123", "Warehouse", "warehouse01" },
                    { 11, "Trương Văn Thủ Kho 2", true, "wh@123", "Warehouse", "warehouse02" },
                    { 12, "Lý Thị Kiểm Kê", true, "wh@123", "Warehouse", "warehouse03" },
                    { 13, "Mai Thị Thu Ngân 8", true, "cashier@123", "Cashier", "cashier08" },
                    { 14, "Dương Thị Thu Ngân 9", true, "cashier@123", "Cashier", "cashier09" },
                    { 15, "Hồ Thị Nhân Viên Nghỉ", false, "lock@123", "Cashier", "staff_lock" }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "OrderDetailId", "OrderId", "ProductId", "ProductName", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, "Tã dán Pampers Newborn size NB – 84 miếng", 1, 195000m },
                    { 2, 1, 4, "Khăn ướt Bobby không mùi hộp 100 tờ", 1, 42000m },
                    { 3, 1, 14, "Sữa tắm gội Pigeon 2 trong 1 200ml", 1, 95000m },
                    { 4, 2, 5, "Sữa bột NAN Optipro 1 hộp 800g", 1, 520000m },
                    { 5, 3, 11, "Váng sữa Petit Suisse Yoplait vị dâu lốc 6 hộp", 3, 65000m },
                    { 6, 3, 9, "Bột ăn dặm Heinz vị gạo sữa 200g", 1, 85000m },
                    { 7, 4, 6, "Sữa bột Enfamil A+ số 2 hộp 900g", 1, 490000m },
                    { 8, 4, 2, "Tã quần Huggies Dry size M – 48 miếng", 1, 185000m },
                    { 9, 5, 12, "Bình sữa Pigeon PPSU cổ rộng 160ml", 1, 185000m },
                    { 10, 6, 18, "Sữa bầu Enfamama A+ hộp 900g", 1, 450000m },
                    { 11, 7, 15, "Kem chống hăm Bepanthen Baby 30g", 1, 125000m },
                    { 12, 8, 7, "Sữa bột Aptamil số 3 hộp 800g", 1, 480000m },
                    { 13, 8, 8, "Sữa bột Dielac Alpha Step 1 hộp 900g", 1, 320000m },
                    { 14, 9, 4, "Khăn ướt Bobby không mùi hộp 100 tờ", 2, 42000m },
                    { 15, 9, 17, "Bộ bodysuit cotton sơ sinh 5 bộ size 0-3 tháng", 1, 280000m },
                    { 16, 10, 10, "Cháo tươi ăn dặm Nutricare gà rau củ 200g", 3, 38000m },
                    { 17, 10, 11, "Váng sữa Petit Suisse Yoplait vị dâu lốc 6 hộp", 2, 65000m },
                    { 18, 12, 5, "Sữa bột NAN Optipro 1 hộp 800g", 1, 520000m },
                    { 19, 12, 3, "Tã dán Bobby Newborn size S – 56 miếng", 1, 135000m },
                    { 20, 12, 13, "Bình sữa Dr. Brown's chống sặc 250ml", 1, 245000m },
                    { 21, 14, 18, "Sữa bầu Enfamama A+ hộp 900g", 1, 450000m },
                    { 22, 14, 19, "Vitamin Blackmores Pregnancy & Breast-feeding 60 viên", 1, 380000m },
                    { 23, 15, 20, "Bộ đồ chơi xúc xắc gặm nướu an toàn cho bé 8 món", 1, 165000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "OrderDetails",
                keyColumn: "OrderDetailId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 50);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                column: "CategoryName",
                value: "Sữa tươi & Sữa dinh dưỡng cho bé");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8,
                column: "CategoryName",
                value: "Vệ sinh & Chăm sóc răng miệng bé");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 13,
                column: "CategoryName",
                value: "Dinh dưỡng & Vitamin cho mẹ bầu");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 14,
                column: "Description",
                value: "Đai bụng, kem rạn da, miếng lót sau sinh, đồ dùng cho mẹ");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Nguyễn Văn A", 150 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Trần Thị B", 50 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Lê Văn C", 10 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4,
                columns: new[] { "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[] { "Phạm Thanh D", "Kim cương", "0234567884", 345 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Hoàng Minh Tuấn", 220 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6,
                column: "CustomerName",
                value: "Đỗ Thị Lan Anh");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 7,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Vũ Quốc Bảo", 25 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 8,
                columns: new[] { "CustomerName", "MembershipRank", "RewardPoints" },
                values: new object[] { "Ngô Thị Hồng", "Kim cương", 410 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 9,
                column: "CustomerName",
                value: "Bùi Đức Thắng");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 10,
                column: "CustomerName",
                value: "Đặng Thị Mai");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 11,
                column: "CustomerName",
                value: "Dương Văn Khoa");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 12,
                columns: new[] { "CustomerName", "RewardPoints" },
                values: new object[] { "Lý Thị Ngọc", 260 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 13,
                column: "CustomerName",
                value: "Trương Văn Hải");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 14,
                column: "MembershipRank",
                value: "Kim cương");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 15,
                column: "CustomerName",
                value: "Huỳnh Gia Bảo");
        }
    }
}
