using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "Address",
                value: "12 Lê Lợi, Quận 1, TP.HCM");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "Address",
                value: "45 Nguyễn Trãi, Quận 5, TP.HCM");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                column: "Address",
                value: "78 Cách Mạng Tháng 8, Quận 3, TP.HCM");

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 4, "23 Trần Hưng Đạo, Quận 1, TP.HCM", "Phạm Thanh D", "Kim cương", "0234567884", 345 },
                    { 5, "56 Võ Văn Tần, Quận 3, TP.HCM", "Hoàng Minh Tuấn", "Vàng", "0905123456", 220 },
                    { 6, "89 Hai Bà Trưng, Quận 1, TP.HCM", "Đỗ Thị Lan Anh", "Bạc", "0912345678", 85 },
                    { 7, "15 Phan Xích Long, Phú Nhuận, TP.HCM", "Vũ Quốc Bảo", "Chuẩn", "0938765432", 25 },
                    { 8, "102 Lý Thường Kiệt, Quận 10, TP.HCM", "Ngô Thị Hồng", "Kim cương", "0977112233", 410 },
                    { 9, "34 Nguyễn Văn Cừ, Quận 5, TP.HCM", "Bùi Đức Thắng", "Vàng", "0966778899", 175 },
                    { 10, "67 Điện Biên Phủ, Bình Thạnh, TP.HCM", "Đặng Thị Mai", "Chuẩn", "0988001122", 40 },
                    { 11, "21 Lạc Long Quân, Quận 11, TP.HCM", "Dương Văn Khoa", "Bạc", "0945667788", 95 },
                    { 12, "90 Quang Trung, Gò Vấp, TP.HCM", "Lý Thị Ngọc", "Vàng", "0933221100", 260 },
                    { 13, "5 Nguyễn Thị Minh Khai, Quận 3, TP.HCM", "Trương Văn Hải", "Chuẩn", "0902334455", 0 },
                    { 14, "143 Tô Hiến Thành, Quận 10, TP.HCM", "Phan Thị Thu Hà", "Kim cương", "0919889977", 380 },
                    { 15, "28 Âu Cơ, Tân Bình, TP.HCM", "Huỳnh Gia Bảo", "Bạc", "0987556644", 60 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "Address",
                value: null);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "Address",
                value: null);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 3,
                column: "Address",
                value: null);
        }
    }
}
