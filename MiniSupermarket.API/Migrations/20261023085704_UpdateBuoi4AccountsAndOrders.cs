using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBuoi4AccountsAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 13,
                column: "CashierUsername",
                value: "cashier05");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 14,
                column: "CashierUsername",
                value: "cashier06");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Nguyễn Quản Trị", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "admin01" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Trần Giám Đốc", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "admin02" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Lê Thu Ngân", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Phạm Bán Hàng", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Hoàng Thu Ngân", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Vũ Thị Quầy", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Đỗ Bán Lẻ", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Ngô Quản Kho", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "Warehouse", "ware01" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Bùi Kiểm Kê", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "Warehouse", "ware02" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Dương Thủ Kho", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "ware03" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Lý Nhập Hàng", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "ware04" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Đặng Hỗ Trợ", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "Admin", "admin_backup" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Hồ Ca Chiều", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "cashier06" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Trương Vận Chuyển", "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "Warehouse", "ware05" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { "Mai Giám Sát", true, "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=", "Admin", "supervisor" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 13,
                column: "CashierUsername",
                value: "cashier07");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 14,
                column: "CashierUsername",
                value: "cashier07");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Nguyễn Thị Quản Trị", "admin@123", "admin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Trần Cửa Hàng Trưởng", "manager@123", "manager01" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Lê Thị Thu Ngân 1", "cashier@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Phạm Thị Thu Ngân 2", "cashier@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Hoàng Thị Thu Ngân 3", "cashier@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Vũ Thị Thu Ngân 4", "cashier@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "FullName", "PasswordHash" },
                values: new object[] { "Đỗ Thị Thu Ngân 5", "cashier@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Ngô Thị Thu Ngân 6", "cashier@123", "Cashier", "cashier06" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Bùi Thị Thu Ngân 7", "cashier@123", "Cashier", "cashier07" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Đặng Văn Thủ Kho 1", "wh@123", "warehouse01" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Trương Văn Thủ Kho 2", "wh@123", "warehouse02" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Lý Thị Kiểm Kê", "wh@123", "Warehouse", "warehouse03" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "FullName", "PasswordHash", "Username" },
                values: new object[] { "Mai Thị Thu Ngân 8", "cashier@123", "cashier08" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "FullName", "PasswordHash", "Role", "Username" },
                values: new object[] { "Dương Thị Thu Ngân 9", "cashier@123", "Cashier", "cashier09" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { "Hồ Thị Nhân Viên Nghỉ", false, "lock@123", "Cashier", "staff_lock" });
        }
    }
}
