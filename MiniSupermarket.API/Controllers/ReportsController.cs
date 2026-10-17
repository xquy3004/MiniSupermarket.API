using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    // DTO doanh thu theo thu ngân
    public class CashierRevenueDto
    {
        public string CashierUsername { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalPointsAwarded { get; set; }
    }

    // DTO top sản phẩm bán chạy (Bài tập 3.2)
    public class TopSellingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")!;
        }

        // =========================================================================
        // BUỔI 4: BÁO CÁO NHANH CHO WINFORMS FORMQUICKREPORT
        // =========================================================================
        // GET: /api/reports/quick?date=2026-10-10
        [HttpGet("quick")]
        public IActionResult GetQuickReport([FromQuery] DateTime date)
        {
            var rnd = new Random(date.Year * 10000 + date.Month * 100 + date.Day);
            int totalOrders = rnd.Next(20, 80);
            decimal totalRevenue = totalOrders * rnd.Next(80, 250) * 1000m;
            string[] items = { "Mì gói", "Nước suối", "Sữa tươi", "Bánh quy", "Dầu ăn" };

            return Ok(new
            {
                date = date.ToString("dd/MM/yyyy"),
                totalOrders,
                totalRevenue,
                bestSeller = items[rnd.Next(items.Length)],
                isSampleData = true
            });
        }

        // =========================================================================
        // BUỔI 5 - TIẾT 3: BÁO CÁO DOANH THU THEO THU NGÂN (STORED PROCEDURE)
        // =========================================================================
        // GET: api/reports/revenue-by-cashier
        [HttpGet("revenue-by-cashier")]
        public IActionResult GetRevenueByCashier()
        {
            var reportList = new List<CashierRevenueDto>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("sp_GetRevenueByCashier", connection))
                {
                    // ĐIỂM QUAN TRỌNG: Thiết lập CommandType là StoredProcedure
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new CashierRevenueDto
                                {
                                    CashierUsername = reader["CashierUsername"]?.ToString() ?? string.Empty,
                                    TotalOrders = Convert.ToInt32(reader["TotalOrders"]),
                                    TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"]),
                                    TotalPointsAwarded = reader["TotalPointsAwarded"] != DBNull.Value
                                        ? Convert.ToInt32(reader["TotalPointsAwarded"])
                                        : 0
                                };
                                reportList.Add(item);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Lỗi thực thi Stored Procedure trên SQL Server: " + ex.Message
                        });
                    }
                }
            }

            return Ok(new
            {
                success = true,
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                totalCashiers = reportList.Count,
                grandTotalRevenue = reportList.Sum(x => x.TotalRevenue),
                data = reportList
            });
        }

        // =========================================================================
        // BÀI TẬP 3.1: BÁO CÁO DOANH THU THEO KHOẢNG NGÀY (CÓ THAM SỐ)
        // =========================================================================
        // GET: api/reports/revenue-by-date-range?fromDate=2026-10-01&toDate=2026-10-31
        [HttpGet("revenue-by-date-range")]
        public IActionResult GetRevenueByDateRange([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            DateTime start = fromDate ?? DateTime.Today.AddDays(-30);
            DateTime end = toDate ?? DateTime.Today.AddDays(1).AddSeconds(-1);

            var reportList = new List<CashierRevenueDto>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("sp_GetRevenueByDateRange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FromDate", start);
                    command.Parameters.AddWithValue("@ToDate", end);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reportList.Add(new CashierRevenueDto
                                {
                                    CashierUsername = reader["CashierUsername"]?.ToString() ?? string.Empty,
                                    TotalOrders = Convert.ToInt32(reader["TotalOrders"]),
                                    TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"]),
                                    TotalPointsAwarded = reader["TotalPointsAwarded"] != DBNull.Value
                                        ? Convert.ToInt32(reader["TotalPointsAwarded"])
                                        : 0
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Lỗi thực thi Stored Procedure: " + ex.Message
                        });
                    }
                }
            }

            return Ok(new
            {
                success = true,
                fromDate = start.ToString("yyyy-MM-dd HH:mm:ss"),
                toDate = end.ToString("yyyy-MM-dd HH:mm:ss"),
                totalCashiers = reportList.Count,
                grandTotalRevenue = reportList.Sum(x => x.TotalRevenue),
                data = reportList
            });
        }

        // =========================================================================
        // BÀI TẬP 3.2: TOP 5 SẢN PHẨM BÁN CHẠY NHẤT
        // =========================================================================
        // GET: api/reports/top-selling
        [HttpGet("top-selling")]
        public IActionResult GetTopSellingProducts()
        {
            var list = new List<TopSellingProductDto>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand("sp_GetTop5SellingProducts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new TopSellingProductDto
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    ProductName = reader["ProductName"]?.ToString() ?? string.Empty,
                                    TotalQuantitySold = Convert.ToInt32(reader["TotalQuantitySold"]),
                                    TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"])
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Lỗi thực thi Stored Procedure: " + ex.Message
                        });
                    }
                }
            }

            return Ok(new
            {
                success = true,
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                totalProducts = list.Count,
                data = list
            });
        }
    }
}