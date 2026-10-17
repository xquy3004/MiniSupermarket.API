using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace ThucHanhAdoNet
{
    internal class Program
    {
        private const string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            while (true)
            {
                if (!Console.IsInputRedirected)
                {
                    try { Console.Clear(); } catch { }
                }
                Console.WriteLine("=========================================================================");
                Console.WriteLine("          HỆ THỐNG THỰC HÀNH ADO.NET NÂNG CAO & GIAO DỊCH (BUỔI 5)       ");
                Console.WriteLine("=========================================================================");
                Console.WriteLine(" 1. [Tiết 1 - BT 1] Đếm tổng số mặt hàng bằng ExecuteScalar()");
                Console.WriteLine(" 2. [Tiết 1 - BT 2] Cập nhật tồn kho sản phẩm bằng ExecuteNonQuery() & SqlParameter");
                Console.WriteLine(" 3. [Tiết 1 - BT 3] Đọc danh sách sản phẩm bằng SqlDataReader");
                Console.WriteLine(" 4. [Tiết 4 - Demo] Kiểm chứng cơ chế Rollback của SqlTransaction (Chuẩn ACID)");
                Console.WriteLine(" 5. Chạy tuần tự tất cả các bài tập trên");
                Console.WriteLine(" 0. Thoát chương trình");
                Console.WriteLine("=========================================================================");
                Console.Write("Vui lòng chọn bài tập thực hành [0-5]: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice?.Trim())
                {
                    case "1":
                        BaiTap1_ExecuteScalar();
                        WaitForKey();
                        break;
                    case "2":
                        BaiTap2_ExecuteNonQuery();
                        WaitForKey();
                        break;
                    case "3":
                        BaiTap3_SqlDataReader();
                        WaitForKey();
                        break;
                    case "4":
                        BaiTap4_SqlTransaction_Rollback();
                        WaitForKey();
                        break;
                    case "5":
                        BaiTap1_ExecuteScalar();
                        Console.WriteLine();
                        BaiTap2_ExecuteNonQuery();
                        Console.WriteLine();
                        BaiTap3_SqlDataReader();
                        Console.WriteLine();
                        BaiTap4_SqlTransaction_Rollback();
                        WaitForKey();
                        break;
                    case "0":
                        Console.WriteLine("Tạm biệt! Kết thúc chương trình.");
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ, vui lòng chọn lại.");
                        WaitForKey();
                        break;
                }
            }
        }

        private static void WaitForKey()
        {
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("\n-> Nhấn phím bất kỳ để tiếp tục...");
                try { Console.ReadKey(); } catch { }
            }
        }

        /// <summary>
        /// BÀI TẬP 1: Đếm tổng số mặt hàng bằng ExecuteScalar()
        /// </summary>
        public static void BaiTap1_ExecuteScalar()
        {
            Console.WriteLine("=== BÀI TẬP 1: ĐẾM TỔNG SỐ MẶT HÀNG BẰNG EXECUTESCALAR ===");
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM Products";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        int totalProducts = Convert.ToInt32(command.ExecuteScalar());
                        Console.WriteLine($"-> KẾT QUẢ: Tổng số mặt hàng hiện có trong siêu thị là: {totalProducts} sản phẩm.");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// BÀI TẬP 2: Cập nhật tồn kho sản phẩm bằng ExecuteNonQuery() và SqlParameter
        /// </summary>
        public static void BaiTap2_ExecuteNonQuery()
        {
            Console.WriteLine("=== BÀI TẬP 2: CẬP NHẬT TỒN KHO AN TOÀN VỚI SQLPARAMETER ===");
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    string query = @"UPDATE Products 
                                     SET StockQuantity = @NewStock 
                                     WHERE ProductId = @Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NewStock", 250);
                        command.Parameters.AddWithValue("@Id", 1);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        Console.WriteLine($"-> THÀNH CÔNG: Đã cập nhật tồn kho cho Sản phẩm ID = 1 thành 250 đơn vị.");
                        Console.WriteLine($"-> Số dòng dữ liệu bị ảnh hưởng: {rowsAffected} dòng.");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// BÀI TẬP 3: Đọc danh sách sản phẩm bằng SqlDataReader
        /// </summary>
        public static void BaiTap3_SqlDataReader()
        {
            Console.WriteLine("=== BÀI TẬP 3: DUYỆT DANH SÁCH BẰNG SQLDATAREADER ===");
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity 
                                     FROM Products 
                                     ORDER BY ProductId ASC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("-----------------------------------------------------------------------------------------");
                            Console.WriteLine(string.Format("{0,-6} | {1,-14} | {2,-35} | {3,12} | {4,8}",
                                "MÃ SP", "MÃ VẠCH", "TÊN SẢN PHẨM", "ĐƠN GIÁ", "TỒN KHO"));
                            Console.WriteLine("-----------------------------------------------------------------------------------------");

                            int stt = 0;
                            while (reader.Read())
                            {
                                stt++;
                                int id = Convert.ToInt32(reader["ProductId"]);
                                string barcode = reader["Barcode"]?.ToString() ?? "";
                                string name = reader["ProductName"]?.ToString() ?? "";
                                decimal price = Convert.ToDecimal(reader["Price"]);
                                int stock = Convert.ToInt32(reader["StockQuantity"]);

                                Console.WriteLine(string.Format("{0,-6} | {1,-14} | {2,-35} | {3,12:N0} đ | {4,8}",
                                    id, barcode, name, price, stock));
                            }
                            Console.WriteLine("-----------------------------------------------------------------------------------------");
                            Console.WriteLine($"-> Tổng cộng đã đọc thành công: {stt} mặt hàng từ SQL Server.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// TIẾT 4: Kiểm chứng cơ chế Rollback của SqlTransaction (Chuẩn ACID)
        /// </summary>
        public static void BaiTap4_SqlTransaction_Rollback()
        {
            Console.WriteLine("=== TIẾT 4: THỬ NGHIỆM ĐIỀU KHIỂN SQLTRANSACTION & KIỂM CHỨNG ROLLBACK ===");
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // Lấy tồn kho hiện tại của SP 1 trước khi chạy Transaction
                int initialStock = 0;
                using (SqlCommand cmdCheck = new SqlCommand("SELECT StockQuantity FROM Products WHERE ProductId = 1", connection))
                {
                    initialStock = Convert.ToInt32(cmdCheck.ExecuteScalar());
                }
                Console.WriteLine($"-> Tồn kho SP 1 trước giao dịch: {initialStock}");

                // 1. Khởi tạo Transaction
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    Console.WriteLine("-> Bắt đầu phiên giao dịch (connection.BeginTransaction())...");

                    // 2. Thao tác 1: Trừ 5 đơn vị tồn kho SP 1 (Hợp lệ)
                    string sqlStep1 = "UPDATE Products SET StockQuantity = StockQuantity - @Qty WHERE ProductId = @Id";
                    using (SqlCommand cmd1 = new SqlCommand(sqlStep1, connection, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@Qty", 5);
                        cmd1.Parameters.AddWithValue("@Id", 1);
                        int r1 = cmd1.ExecuteNonQuery();
                        Console.WriteLine($"-> Bước 1: Đã thực thi trừ tồn kho SP 1 (Số dòng tác động: {r1}).");
                    }

                    // 3. Thao tác 2: Giả lập lỗi ngoại lệ (hết kho thực tế)
                    Console.WriteLine("-> Bước 2: Chuẩn bị thực thi thao tác kiểm tra mặt hàng thứ 2...");
                    bool isInventoryShortage = true; // Giả lập mặt hàng thứ 2 bị hết tồn kho
                    if (isInventoryShortage)
                    {
                        throw new InvalidOperationException("Mặt hàng thứ hai đã hết tồn kho thực tế! Giao dịch không thể hoàn tất.");
                    }

                    // 4. Nếu không có lỗi thì mới Commit
                    transaction.Commit();
                    Console.WriteLine("-> GIAO DỊCH COMMIT THÀNH CÔNG!");
                }
                catch (Exception ex)
                {
                    // 5. Khi bắt được lỗi -> Lập tức Rollback
                    Console.WriteLine($"\n[PHÁT SINH LỖI]: {ex.Message}");
                    transaction.Rollback();
                    Console.WriteLine("-> [HỆ THỐNG]: Đã kích hoạt transaction.Rollback() thành công!");
                    Console.WriteLine("-> Toàn bộ thay đổi của Bước 1 đã được hoàn tác sạch sẽ.");
                }

                // Kiểm tra lại tồn kho SP 1 sau Rollback
                int postStock = 0;
                using (SqlCommand cmdCheck = new SqlCommand("SELECT StockQuantity FROM Products WHERE ProductId = 1", connection))
                {
                    postStock = Convert.ToInt32(cmdCheck.ExecuteScalar());
                }
                Console.WriteLine($"-> Tồn kho SP 1 sau khi Rollback: {postStock} (Khớp hoàn toàn với trước giao dịch: {postStock == initialStock}).");
                Console.WriteLine("-> [KẾT LUẬN]: Dữ liệu đảm bảo trọn vẹn tính Nguyên tử (Atomicity) theo chuẩn ACID!");
            }
        }
    }
}
