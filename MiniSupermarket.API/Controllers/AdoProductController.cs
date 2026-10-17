using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    // DTO tra cứu sản phẩm cho quầy POS
    public class ProductLookupDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    // DTO đếm sản phẩm theo danh mục (Bài tập 2.2)
    public class CategoryProductCountDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
    }

    [Route("api/ado-products")]
    [ApiController]
    public class AdoProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Tiêm IConfiguration để đọc chuỗi kết nối từ appsettings.json
        public AdoProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")!;
        }

        // =========================================================================
        // THAO TÁC 1: TRA CỨU THEO MÃ VẠCH (CONNECTED MODE - SQLDATAREADER)
        // =========================================================================
        // GET: api/ado-products/barcode/893456789001
        [HttpGet("barcode/{barcode}")]
        public IActionResult GetProductByBarcode(string barcode)
        {
            // 1. Kiểm tra tính hợp lệ của tham số đầu vào
            if (string.IsNullOrWhiteSpace(barcode))
            {
                return BadRequest(new { message = "Mã vạch không được để trống!" });
            }

            // 2. Sử dụng khối using để đảm bảo SqlConnection luôn được đóng và giải phóng
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                // 3. Câu lệnh SELECT có tham số hóa @Barcode chống SQL Injection
                string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity 
                                 FROM Products 
                                 WHERE Barcode = @Barcode";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // 4. Gắn giá trị an toàn qua SqlParameter
                    command.Parameters.AddWithValue("@Barcode", barcode.Trim());

                    try
                    {
                        // 5. Mở kết nối vật lý tới SQL Server
                        connection.Open();

                        // 6. Khởi tạo luồng đọc nhanh Forward-Only SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // 7. Đọc bản ghi: reader.Read() trả về true nếu tìm thấy dòng dữ liệu
                            if (reader.Read())
                            {
                                var productDto = new ProductLookupDto
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    Barcode = reader["Barcode"]?.ToString() ?? string.Empty,
                                    ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    StockQuantity = Convert.ToInt32(reader["StockQuantity"])
                                };

                                return Ok(productDto);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
                    }
                }
            } // Kết thúc using, connection.Close() tự động được thực thi tại đây

            // 8. Nếu không tìm thấy sản phẩm nào khớp mã vạch -> Trả về 404 Not Found
            return NotFound(new { message = $"Không tìm thấy sản phẩm có mã vạch: {barcode}" });
        }

        // =========================================================================
        // THAO TÁC 2: TẢI DANH SÁCH LƯU ĐỆM (DISCONNECTED MODE - DATATABLE & ADAPTER)
        // =========================================================================
        // GET: api/ado-products/disconnected-cache
        [HttpGet("disconnected-cache")]
        public IActionResult GetProductsDisconnected()
        {
            // 1. Khởi tạo bảng ảo DataTable trên RAM
            DataTable productTable = new DataTable("CachedProducts");

            // 2. Mở kết nối và sử dụng SqlDataAdapter để nạp dữ liệu
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity 
                                 FROM Products 
                                 ORDER BY ProductId ASC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // 3. SqlDataAdapter làm cầu nối giữa Command và DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        // 4. Phương thức Fill(): Tự động Open -> Nạp dữ liệu vào DataTable -> Tự động Close connection
                        adapter.Fill(productTable);
                    }
                }
            } // Tại đây, kết nối CSDL ĐÃ ĐƯỢC ĐÓNG HOÀN TOÀN!

            // 5. Thao tác độc lập trên bộ nhớ RAM (Không cần kết nối tới SQL Server nữa)
            var resultList = new List<ProductLookupDto>();
            foreach (DataRow row in productTable.Rows)
            {
                resultList.Add(new ProductLookupDto
                {
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Barcode = row["Barcode"]?.ToString() ?? string.Empty,
                    ProductName = row["ProductName"]?.ToString() ?? string.Empty,
                    Price = Convert.ToDecimal(row["Price"]),
                    StockQuantity = Convert.ToInt32(row["StockQuantity"])
                });
            }

            return Ok(new
            {
                totalLoaded = productTable.Rows.Count,
                cachedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                data = resultList
            });
        }

        // =========================================================================
        // BÀI TẬP 2.1: TRA CỨU THEO KHOẢNG TỒN KHO (SQLDATAREADER CẢNH BÁO NHẬP HÀNG)
        // =========================================================================
        // GET: api/ado-products/filter-stock?minStock=50
        [HttpGet("filter-stock")]
        public IActionResult FilterStock([FromQuery] int minStock = 50)
        {
            var resultList = new List<ProductLookupDto>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity 
                                 FROM Products 
                                 WHERE StockQuantity <= @MinStock 
                                 ORDER BY StockQuantity ASC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MinStock", minStock);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(new ProductLookupDto
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    Barcode = reader["Barcode"]?.ToString() ?? string.Empty,
                                    ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    StockQuantity = Convert.ToInt32(reader["StockQuantity"])
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
                    }
                }
            }

            return Ok(new
            {
                minStockThreshold = minStock,
                totalLowStockProducts = resultList.Count,
                data = resultList
            });
        }

        // =========================================================================
        // BÀI TẬP 2.2: ĐẾM SỐ LƯỢNG SẢN PHẨM THEO TỪNG NHÓM DANH MỤC (DATATABLE)
        // =========================================================================
        // GET: api/ado-products/category-count
        [HttpGet("category-count")]
        public IActionResult GetProductCountByCategory()
        {
            DataTable categoryTable = new DataTable("CategoryStats");

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                string query = @"SELECT c.CategoryId, c.CategoryName, COUNT(p.ProductId) AS TotalProducts 
                                 FROM Categories c 
                                 LEFT JOIN Products p ON c.CategoryId = p.CategoryId 
                                 GROUP BY c.CategoryId, c.CategoryName 
                                 ORDER BY c.CategoryId ASC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(categoryTable);
                    }
                }
            }

            var result = new List<CategoryProductCountDto>();
            foreach (DataRow row in categoryTable.Rows)
            {
                result.Add(new CategoryProductCountDto
                {
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    CategoryName = row["CategoryName"]?.ToString() ?? string.Empty,
                    TotalProducts = Convert.ToInt32(row["TotalProducts"])
                });
            }

            return Ok(new
            {
                totalCategories = result.Count,
                data = result
            });
        }
    }
}
