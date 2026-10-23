using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
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

    // DTO top sản phẩm bán chạy
    public class TopSellingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // DTO doanh thu theo thời gian / ngày
    public class DateRevenueDto
    {
        public string TimePeriod { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalPointsAwarded { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SupermarketDbContext _context;

        public ReportsController(IConfiguration configuration, SupermarketDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // =========================================================================
        // BÁO CÁO DOANH THU NHANH (100% DỮ LIỆU THẬT TỪ BẢNG ORDERS & ORDERDETAILS)
        // =========================================================================
        // GET: /api/reports/quick?fromDate=2026-10-01&toDate=2026-10-31 OR ?date=2026-10-23
        [HttpGet("quick")]
        public async Task<IActionResult> GetQuickReport(
            [FromQuery] DateTime? date,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            DateTime start;
            DateTime end;

            if (fromDate.HasValue && toDate.HasValue)
            {
                start = fromDate.Value.Date;
                end = toDate.Value.Date.AddDays(1).AddTicks(-1);
            }
            else if (date.HasValue)
            {
                start = date.Value.Date;
                end = date.Value.Date.AddDays(1).AddTicks(-1);
            }
            else
            {
                // Không truyền ngày => Lấy toàn bộ lịch sử dữ liệu thật
                start = DateTime.MinValue;
                end = DateTime.MaxValue;
            }

            var orderQuery = _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= start && o.OrderDate <= end);

            int totalOrders = await orderQuery.CountAsync();
            decimal totalRevenue = totalOrders > 0 ? await orderQuery.SumAsync(o => o.TotalAmount) : 0;
            int totalPoints = totalOrders > 0 ? await orderQuery.SumAsync(o => o.PointsEarned) : 0;
            decimal avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Sản phẩm bán chạy nhất trong khoảng thời gian này
            var bestSellerItem = await _context.OrderDetails
                .AsNoTracking()
                .Where(d => d.Order != null && d.Order.OrderDate >= start && d.Order.OrderDate <= end)
                .GroupBy(d => d.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.Quantity)
                .FirstOrDefaultAsync();

            string bestSeller = bestSellerItem != null
                ? $"{bestSellerItem.ProductName} ({bestSellerItem.Quantity} sản phẩm)"
                : "Chưa có giao dịch";

            return Ok(new
            {
                date = (date ?? DateTime.Today).ToString("dd/MM/yyyy"),
                fromDate = start == DateTime.MinValue ? "" : start.ToString("dd/MM/yyyy"),
                toDate = end == DateTime.MaxValue ? "" : end.ToString("dd/MM/yyyy"),
                totalOrders,
                totalRevenue,
                averageOrderValue = avgOrderValue,
                totalPointsAwarded = totalPoints,
                bestSeller,
                isSampleData = false // DỮ LIỆU THẬT 100%
            });
        }

        // =========================================================================
        // BÁO CÁO DOANH THU THEO THU NGÂN (THỰC TẾ)
        // =========================================================================
        // GET: api/reports/revenue-by-cashier?fromDate=...&toDate=...
        [HttpGet("revenue-by-cashier")]
        public async Task<IActionResult> GetRevenueByCashier(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var orderQuery = _context.Orders.AsNoTracking();

            if (fromDate.HasValue)
            {
                var start = fromDate.Value.Date;
                orderQuery = orderQuery.Where(o => o.OrderDate >= start);
            }
            if (toDate.HasValue)
            {
                var end = toDate.Value.Date.AddDays(1).AddTicks(-1);
                orderQuery = orderQuery.Where(o => o.OrderDate <= end);
            }

            var reportList = await orderQuery
                .GroupBy(o => o.CashierUsername)
                .Select(g => new CashierRevenueDto
                {
                    CashierUsername = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    TotalPointsAwarded = g.Sum(o => o.PointsEarned)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

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
        // BÁO CÁO DOANH THU THEO KHOẢNG NGÀY / THỜI GIAN
        // =========================================================================
        // GET: api/reports/revenue-by-date-range?fromDate=2026-10-01&toDate=2026-10-31
        [HttpGet("revenue-by-date-range")]
        public async Task<IActionResult> GetRevenueByDateRange([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var orderQuery = _context.Orders.AsNoTracking();

            if (fromDate.HasValue)
            {
                var start = fromDate.Value.Date;
                orderQuery = orderQuery.Where(o => o.OrderDate >= start);
            }
            if (toDate.HasValue)
            {
                var end = toDate.Value.Date.AddDays(1).AddTicks(-1);
                orderQuery = orderQuery.Where(o => o.OrderDate <= end);
            }

            var orders = await orderQuery.ToListAsync();

            bool isSingleDay = fromDate.HasValue && toDate.HasValue && fromDate.Value.Date == toDate.Value.Date;

            List<DateRevenueDto> reportList;

            if (isSingleDay)
            {
                // Nhóm theo khung giờ trong ngày
                reportList = orders
                    .GroupBy(o => $"{o.OrderDate:HH:00} - {o.OrderDate.AddHours(1):HH:00}")
                    .Select(g => new DateRevenueDto
                    {
                        TimePeriod = g.Key,
                        TotalOrders = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalAmount),
                        AverageOrderValue = g.Count() > 0 ? g.Sum(o => o.TotalAmount) / g.Count() : 0,
                        TotalPointsAwarded = g.Sum(o => o.PointsEarned)
                    })
                    .OrderBy(x => x.TimePeriod)
                    .ToList();
            }
            else
            {
                // Nhóm theo từng ngày
                reportList = orders
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new DateRevenueDto
                    {
                        TimePeriod = g.Key.ToString("dd/MM/yyyy"),
                        TotalOrders = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalAmount),
                        AverageOrderValue = g.Count() > 0 ? g.Sum(o => o.TotalAmount) / g.Count() : 0,
                        TotalPointsAwarded = g.Sum(o => o.PointsEarned)
                    })
                    .OrderByDescending(x => DateTime.ParseExact(x.TimePeriod, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                    .ToList();
            }

            return Ok(new
            {
                success = true,
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                totalPeriods = reportList.Count,
                grandTotalRevenue = reportList.Sum(x => x.TotalRevenue),
                data = reportList
            });
        }

        // =========================================================================
        // BÁO CÁO TOP SẢN PHẨM BÁN CHẠY NHẤT (THỰC TẾ)
        // =========================================================================
        // GET: api/reports/top-selling?fromDate=...&toDate=...&limit=10
        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSellingProducts(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int limit = 10)
        {
            var detailQuery = _context.OrderDetails.AsNoTracking();

            if (fromDate.HasValue)
            {
                var start = fromDate.Value.Date;
                detailQuery = detailQuery.Where(d => d.Order != null && d.Order.OrderDate >= start);
            }
            if (toDate.HasValue)
            {
                var end = toDate.Value.Date.AddDays(1).AddTicks(-1);
                detailQuery = detailQuery.Where(d => d.Order != null && d.Order.OrderDate <= end);
            }

            var list = await detailQuery
                .GroupBy(d => new { d.ProductId, d.ProductName })
                .Select(g => new TopSellingProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(limit > 0 ? limit : 10)
                .ToListAsync();

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
