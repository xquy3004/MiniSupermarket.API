using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBabyCareCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Sữa bột & Sữa công thức", "Sữa bột cho bé theo độ tuổi, sữa công thức đặc biệt" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Sữa tươi & Sữa dinh dưỡng cho bé", "Sữa tươi, sữa chua uống, sữa hạt cho trẻ nhỏ" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Bỉm, Tã & Khăn ướt", "Bỉm dán, tã quần, khăn ướt, giấy ướt cho bé" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Đồ ăn dặm & Bột ăn dặm", "Bột ăn dặm, cháo tươi, bánh ăn dặm, váng sữa" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Bình sữa & Núm ty", "Bình sữa, núm ty, ti giả, dụng cụ vệ sinh bình sữa" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 6, "Đồ dùng ăn uống cho bé", "Chén, muỗng, ly tập uống, yếm ăn, ghế ăn dặm" },
                    { 7, "Tắm gội & Chăm sóc da bé", "Sữa tắm, dầu gội, kem chống hăm, phấn rôm, dầu massage" },
                    { 8, "Vệ sinh & Chăm sóc răng miệng bé", "Bàn chải, kem đánh răng, tưa lưỡi, nhiệt kế, bấm móng tay" },
                    { 9, "Quần áo & Phụ kiện sơ sinh", "Bodysuit, đồ sơ sinh, tất, mũ, bao tay, bao chân" },
                    { 10, "Chăn, Gối & Đồ ngủ", "Chăn, gối chặn, nệm, màn, khăn quấn ủ cho bé" },
                    { 11, "Đồ chơi & Học liệu", "Đồ chơi gặm nướu, xúc xắc, sách vải, đồ chơi phát triển trí tuệ" },
                    { 12, "Xe đẩy, Địu & Ghế ngồi ô tô", "Xe đẩy, địu em bé, nôi, ghế ngồi ô tô an toàn" },
                    { 13, "Dinh dưỡng & Vitamin cho mẹ bầu", "Sữa bầu, vitamin tổng hợp, canxi, sắt, DHA cho mẹ" },
                    { 14, "Chăm sóc mẹ sau sinh", "Đai bụng, kem rạn da, miếng lót sau sinh, đồ dùng cho mẹ" },
                    { 15, "Máy hút sữa & Trữ sữa", "Máy hút sữa, túi trữ sữa, miếng lót thấm sữa, phễu hút" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Bánh kẹo & Đồ ăn vặt", "Snack, bánh quy, kẹo dẻo" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Nước giải khát & Trà", "Nước ngọt, nước khoáng, trà" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Sữa & Sản phẩm từ sữa", "Sữa tươi, sữa chua, phô mai" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Mì gói & Thực phẩm ăn liền", "Mì ăn liền, phở khô, cháo gói" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "CategoryName", "Description" },
                values: new object[] { "Gia vị & Dầu ăn", "Nước mắm, hạt nêm, dầu thực vật" });
        }
    }
}
