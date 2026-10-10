using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Cashier")]   // Warehouse không được bán hàng
    public class OrdersController : ControllerBase
    {
        // Quy đổi điểm thưởng: cứ 10.000đ được 1 điểm
        private const decimal VndPerPoint = 10000m;

        private readonly SupermarketDbContext _context;

        public OrdersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // POST /api/orders -> thanh toán giỏ hàng
        // Client chỉ gửi ProductId + Quantity; đơn giá và tổng tiền luôn do server tính.
        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            if (request?.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new { message = "Giỏ hàng đang trống!" });
            }

            if (request.Items.Any(i => i.Quantity <= 0))
            {
                return BadRequest(new { message = "Số lượng mỗi mặt hàng phải lớn hơn 0!" });
            }

            // Gộp các dòng trùng sản phẩm
            var lines = request.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            var ids = lines.Select(l => l.ProductId).ToList();
            var products = await _context.Products.AsNoTracking()
                .Where(p => ids.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId);

            if (products.Count != ids.Count)
            {
                return BadRequest(new { message = "Có sản phẩm trong giỏ không còn tồn tại!" });
            }

            Customer? customer = null;
            if (request.CustomerId.HasValue && request.CustomerId.Value > 0)
            {
                customer = await _context.Customers.FindAsync(request.CustomerId.Value);
                if (customer == null)
                {
                    return BadRequest(new { message = "Khách hàng không tồn tại!" });
                }
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            var order = new Order
            {
                OrderDate = DateTime.Now,
                CashierUsername = User.Identity?.Name ?? "unknown",
                CustomerId = customer?.CustomerId
            };

            decimal total = 0;
            foreach (var line in lines)
            {
                var product = products[line.ProductId];
                int qty = line.Quantity;
                int productId = line.ProductId;

                // Trừ kho bằng một câu UPDATE có điều kiện (StockQuantity >= qty)
                int affected = await _context.Products
                    .Where(p => p.ProductId == productId && p.StockQuantity >= qty)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockQuantity, p => p.StockQuantity - qty));

                if (affected == 0)
                {
                    // Thoát mà không Commit => transaction tự rollback
                    return BadRequest(new
                    {
                        message = $"Sản phẩm \"{product.ProductName}\" không đủ tồn kho (cần {qty}, hiện có {product.StockQuantity})!"
                    });
                }

                order.Details.Add(new OrderDetail
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.Price,
                    Quantity = qty
                });
                total += product.Price * qty;
            }

            order.TotalAmount = total;

            // Tích điểm và xét hạng cho khách thành viên
            if (customer != null)
            {
                order.PointsEarned = (int)(total / VndPerPoint);
                customer.RewardPoints += order.PointsEarned;
                customer.MembershipRank = RankFor(customer.RewardPoints);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, ToResponse(order, customer?.CustomerName));
        }

        // GET /api/orders/5 -> xem lại một hóa đơn
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders.AsNoTracking()
                .Include(o => o.Details)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn!" });
            }

            // Thu ngân chỉ xem được hóa đơn do chính mình lập
            bool isAdmin = User.IsInRole("Admin");
            if (!isAdmin && !string.Equals(order.CashierUsername, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            return Ok(ToResponse(order, order.Customer?.CustomerName));
        }

        // GET /api/orders?date=2026-10-10 -> danh sách hóa đơn trong ngày (chỉ Admin)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime? date)
        {
            var day = (date ?? DateTime.Today).Date;
            var next = day.AddDays(1);

            var orders = await _context.Orders.AsNoTracking()
                .Include(o => o.Details)
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= day && o.OrderDate < next)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders.Select(o => ToResponse(o, o.Customer?.CustomerName)));
        }

        // Hạng thành viên theo điểm
        private static string RankFor(int points)
        {
            if (points >= 100) return "Vàng";
            if (points >= 50) return "Bạc";
            return "Chuẩn";
        }

        private static OrderResponse ToResponse(Order o, string? customerName)
        {
            return new OrderResponse
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CashierUsername = o.CashierUsername,
                CustomerName = customerName ?? "Khách lẻ",
                TotalAmount = o.TotalAmount,
                PointsEarned = o.PointsEarned,
                Items = o.Details.Select(d => new OrderItemResponse
                {
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.UnitPrice * d.Quantity
                }).ToList()
            };
        }
    }

    // ===== DTO nhận từ client =====
    public class CheckoutRequest
    {
        public int? CustomerId { get; set; }
        public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();
    }

    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    // ===== DTO trả về client =====
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CashierUsername { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int PointsEarned { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }

    public class OrderItemResponse
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}