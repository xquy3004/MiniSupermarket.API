using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Helpers;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Toàn bộ controller chỉ dành cho Admin
    public class UsersController : ControllerBase
    {
        private static readonly string[] ValidRoles = { "Admin", "Cashier", "Warehouse" };

        private readonly SupermarketDbContext _context;

        public UsersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // 1. GET /api/users : danh sách tài khoản (không trả mật khẩu)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Users.AsNoTracking()
                .OrderBy(u => u.Id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Role = u.Role,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Ok(list);
        }

        // 2. POST /api/users : tạo tài khoản mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            string username = (request.Username ?? string.Empty).Trim();
            string fullName = (request.FullName ?? string.Empty).Trim();

            if (username.Length == 0 || username.Length > 50)
            {
                return BadRequest(new { message = "Tên đăng nhập phải từ 1 đến 50 ký tự!" });
            }

            if (fullName.Length == 0 || fullName.Length > 100)
            {
                return BadRequest(new { message = "Họ tên phải từ 1 đến 100 ký tự!" });
            }

            if ((request.Password ?? string.Empty).Length < 6)
            {
                return BadRequest(new { message = "Mật khẩu tối thiểu 6 ký tự!" });
            }

            if (string.IsNullOrEmpty(request.Role) || !ValidRoles.Contains(request.Role))
            {
                return BadRequest(new { message = "Vai trò không hợp lệ (Admin, Cashier hoặc Warehouse)!" });
            }

            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                return Conflict(new { message = "Tên đăng nhập đã tồn tại!" });
            }

            var user = new AppUser
            {
                Username = username,
                FullName = fullName,
                Role = request.Role,
                PasswordHash = PasswordHelper.Hash(request.Password!),
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            });
        }

        // 3. PUT /api/users/5/reset-password : đặt lại mật khẩu
        [HttpPut("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy tài khoản!" });
            }

            if ((request.NewPassword ?? string.Empty).Length < 6)
            {
                return BadRequest(new { message = "Mật khẩu tối thiểu 6 ký tự!" });
            }

            user.PasswordHash = PasswordHelper.Hash(request.NewPassword!);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã đặt lại mật khẩu!" });
        }

        // 4. PUT /api/users/5/toggle-lock : khóa / mở khóa tài khoản
        [HttpPut("{id:int}/toggle-lock")]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy tài khoản!" });
            }

            // Không cho Admin tự khóa chính mình (tránh bị khóa khỏi hệ thống)
            if (string.Equals(user.Username, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Không thể tự khóa tài khoản đang đăng nhập!" });
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                isActive = user.IsActive,
                message = user.IsActive ? "Đã mở khóa tài khoản!" : "Đã khóa tài khoản!"
            });
        }
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}