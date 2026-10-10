using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]   // Chỉ Admin được gọi, Cashier/Warehouse sẽ nhận 403
    public class ReportsController : ControllerBase
    {
        // GET: /api/reports/quick?date=2026-10-10
        [HttpGet("quick")]
        public IActionResult GetQuickReport([FromQuery] DateTime date)
        {
            // Dữ liệu MẪU, ổn định theo ngày (chưa có bảng hóa đơn)
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
    }
}