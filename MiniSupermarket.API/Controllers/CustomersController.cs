using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public CustomersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // 1. GET: Lấy toàn bộ danh sách khách hàng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Customers.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        // 2. GET: Lấy chi tiết khách hàng theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng!" });
            }
            return Ok(customer);
        }

        // 3. SEARCH: Tìm theo tên hoặc số điện thoại
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm!" });
            }

            var result = await _context.Customers
                .Where(c => c.CustomerName.Contains(keyword) || c.PhoneNumber.Contains(keyword))
                .ToListAsync();

            return Ok(result);
        }

        // 4. POST: Thêm khách hàng mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer newCus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(newCus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newCus.CustomerId }, newCus);
        }

        // 5. PUT: Cập nhật khách hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer updateCus)
        {
            var cus = await _context.Customers.FindAsync(id);
            if (cus == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng cần sửa!" });
            }

            cus.CustomerName = updateCus.CustomerName;
            cus.PhoneNumber = updateCus.PhoneNumber;
            cus.Address = updateCus.Address;
            cus.RewardPoints = updateCus.RewardPoints;
            cus.MembershipRank = updateCus.MembershipRank;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE: Xóa khách hàng
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var cus = await _context.Customers.FindAsync(id);
            if (cus == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng cần xóa!" });
            }

            _context.Customers.Remove(cus);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}